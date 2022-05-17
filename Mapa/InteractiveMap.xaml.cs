using Mapa.Services;

namespace Mapa;

public partial class InteractiveMap : ContentView
{
    public InteractiveMap()
    {
        InitializeComponent();

        var httpServer = new HttpServer();
        httpServer.Prefixes.Add("http://localhost:50000/");
        httpServer.StaticFilesPath = Path.Combine(AppContext.BaseDirectory, "InteractiveMap");
        httpServer.Start();

        mapWebView.Source = "http://localhost:50000/";
    }
}