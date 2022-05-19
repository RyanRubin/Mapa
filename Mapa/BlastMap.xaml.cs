using Mapa.Services;

namespace Mapa;

public partial class BlastMap : ContentView
{
    private string url;

    public BlastMap()
    {
        InitializeComponent();

        var httpServer = new HttpServer();
        url = httpServer.Start("localhost", 50000, 60000, Path.Combine(AppContext.BaseDirectory, "BlastMap"));

        mapWebView.Source = url;
    }

    private async void mapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        if (!string.Equals(e.Url, url, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        decimal blastLat = 41.469741498180625m;
        decimal blastLon = -81.49559810202766m;
        decimal npsLat = 41.469m;
        decimal npsLon = -81.495m;
        decimal seismographLat = 41.470m;
        decimal seismographLon = -81.496m;

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({blastLat}, {blastLon}, 'blue')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addIcon({blastLat}, {blastLon}, 'img/icon.png')");

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({npsLat}, {npsLon}, 'yellow')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addPopover({npsLat}, {npsLon}, 'yellow')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addLine({blastLat}, {blastLon}, {npsLat}, {npsLon}, 'yellow')");

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({seismographLat}, {seismographLon}, 'red')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addPopover({seismographLat}, {seismographLon}, 'red')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addLine({blastLat}, {blastLon}, {seismographLat}, {seismographLon}, 'red')");

        await mapWebView.EvaluateJavaScriptAsync("app.fitToFeaturesExtent()");
    }
}