using Mapa.Services;

namespace Mapa;

public partial class BlastMap : ContentView
{
    public BlastMap()
    {
        InitializeComponent();

        var httpServer = new HttpServer();
        string url = httpServer.Start("localhost", 50000, 60000, Path.Combine(AppContext.BaseDirectory, "BlastMap"));

        mapWebView.Source = url;
    }
}