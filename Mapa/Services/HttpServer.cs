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

    public List<string> Prefixes { get; set; } = new();
    public string StaticFilesPath { get; set; }

    public void Start()
    {
        cancelSource = new CancellationTokenSource();
        var thread = new Thread(async (obj) =>
        {
            var listener = new HttpListener();
            foreach (string prefix in Prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
            listener.Start();
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

                    string localPath = Path.Combine(StaticFilesPath, requestPath == "" ? "index.html" : requestPath);
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
    }

    public void Stop()
    {
        cancelSource.Cancel();
    }
}
