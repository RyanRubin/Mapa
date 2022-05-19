using System.Net;

namespace Mapa.Services;

public class HttpServer : IHttpServer
{
    // add mime types as needed
    private readonly Dictionary<string, string> mimeTypes = new() {
        { "css", "text/css" },
        { "html", "text/html" },
        { "js", "text/javascript" },
        { "png", "image/png" }
    };

    private CancellationTokenSource cancelSource;

    private bool isRunning;
    public bool IsRunning { get => isRunning; }

    private string url;
    public string Url { get => url; }

    public void Start(string prefixDomain, int prefixPortMin, int prefixPortMax, string staticFilesPath)
    {
        bool hasError;
        int retryCount = 0;
        var ports = Enumerable.Range(prefixPortMin, prefixPortMax - prefixPortMin).ToList();
        var random = new Random();
        HttpListener listener;
        do
        {
            hasError = false;
            int portIndex = random.Next(0, ports.Count);
            int port = ports[portIndex];
            ports.RemoveAt(portIndex);
            url = $"http://{prefixDomain}:{port}/";
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            try
            {
                listener.Start();
                isRunning = true;
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode == 32) // port already used
                {
                    hasError = true;
                    retryCount++;
                    if (retryCount > 10 || ports.Count == 0)
                    {
                        throw;
                    }
                }
            }
        } while (hasError);

        cancelSource = new CancellationTokenSource();
        var thread = new Thread(async (obj) =>
        {
            var cancelToken = (CancellationToken)obj;
            while (!cancelToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    var context = (HttpListenerContext)obj;
                    var request = context.Request;
                    string requestPath = request.Url.AbsolutePath.TrimStart('/');
                    var response = context.Response;

                    byte[] outputBuffer = Array.Empty<byte>();

                    if (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = Path.Combine(staticFilesPath, requestPath == "" ? "index.html" : requestPath);
                        if (File.Exists(localPath))
                        {
                            outputBuffer = File.ReadAllBytes(localPath);

                            response.ContentLength64 = outputBuffer.Length;
                            response.ContentType = mimeTypes[Path.GetExtension(localPath).TrimStart('.')];
                            response.StatusCode = 200;
                        }
                        else
                        {
                            response.StatusCode = 404;
                        }
                    }
                    else
                    {
                        response.StatusCode = 405; // method not allowed
                    }

                    using var outputStream = response.OutputStream;
                    outputStream.Write(outputBuffer, 0, outputBuffer.Length);
                }, context);
            }

            listener.Stop();
            isRunning = false;
        });
        thread.Start(cancelSource.Token);
    }

    public void Stop()
    {
        cancelSource.Cancel();
    }
}
