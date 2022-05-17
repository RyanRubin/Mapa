using System.Net;

namespace Mapa.Services;

public class HttpServer
{
    // add mime types as needed
    private readonly Dictionary<string, string> mimeTypes = new() {
        { "css", "text/css" },
        { "html", "text/html" },
        { "js", "text/javascript" },
        { "png", "image/png" }
    };

    private CancellationTokenSource cancelSource;

    public string Start(string prefixDomain, int prefixPortMin, int prefixPortMax, string staticFilesPath)
    {
        bool hasError;
        int retryCount = 0;
        var random = new Random();
        string url;
        HttpListener listener;
        do
        {
            hasError = false;
            int port = random.Next(prefixPortMin, prefixPortMax + 1);
            url = $"http://{prefixDomain}:{port}/";
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode == 32) // port already used
                {
                    hasError = true;
                    retryCount++;
                    if (retryCount > 10)
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

                    using var outputStream = response.OutputStream;
                    outputStream.Write(outputBuffer, 0, outputBuffer.Length);
                }, context);
            }

            listener.Stop();
        });
        thread.Start(cancelSource.Token);

        return url;
    }

    public void Stop()
    {
        cancelSource.Cancel();
    }
}
