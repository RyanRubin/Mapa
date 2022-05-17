using Mapa.Services;

namespace Mapa;

public partial class InteractiveMap : ContentView
{
    public InteractiveMap()
    {
        InitializeComponent();

        var httpServer = new HttpServer();
        string url = httpServer.Start("localhost", 50000, 60000, Path.Combine(AppContext.BaseDirectory, "InteractiveMap"));

        mapWebView.Source = url;
    }
}