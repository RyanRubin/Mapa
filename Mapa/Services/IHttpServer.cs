namespace Mapa.Services;

public interface IHttpServer
{
    bool IsRunning { get; }
    string Url { get; }

    void Start(string prefixDomain, int prefixPortMin, int prefixPortMax, string staticFilesPath);
    void Stop();
}