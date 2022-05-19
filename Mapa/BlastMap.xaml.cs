using Mapa.Services;

namespace Mapa;

public partial class BlastMap : ContentView
{
    private const string BlastColor = "blue";
    private const string BlastIcon = "img/icon.png";
    private const string NpsColor = "yellow";
    private const string SeismographColor = "red";

    private readonly IHttpServer httpServer;

    public BlastMap()
    {
        InitializeComponent();

        httpServer = new HttpServer();

        if (!httpServer.IsRunning)
        {
            const int BlastMapServerPortMin = 50000;
            const int BlastMapServerPortMax = 60000;
            const string BlastMapServerStaticFilesPath = "BlastMap";
            httpServer.Start("localhost", BlastMapServerPortMin, BlastMapServerPortMax, Path.Combine(AppContext.BaseDirectory, BlastMapServerStaticFilesPath));
        }

        mapWebView.Source = httpServer.Url;
    }

    private async void mapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        if (!string.Equals(e.Url, httpServer.Url, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // TODO remove dummy data
        decimal blastLat = 41.469741498180625m;
        decimal blastLon = -81.49559810202766m;
        decimal npsLat = 41.469m;
        decimal npsLon = -81.495m;
        decimal seismographLat = 41.470m;
        decimal seismographLon = -81.496m;

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({blastLat}, {blastLon}, '{BlastColor}')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addIcon({blastLat}, {blastLon}, '{BlastIcon}')");

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({npsLat}, {npsLon}, '{NpsColor}')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addPopover({npsLat}, {npsLon}, '{NpsColor}')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addLine({blastLat}, {blastLon}, {npsLat}, {npsLon}, '{NpsColor}')");

        await mapWebView.EvaluateJavaScriptAsync($"app.addCircle({seismographLat}, {seismographLon}, '{SeismographColor}')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addPopover({seismographLat}, {seismographLon}, '{SeismographColor}')");
        await mapWebView.EvaluateJavaScriptAsync($"app.addLine({blastLat}, {blastLon}, {seismographLat}, {seismographLon}, '{SeismographColor}')");

        await mapWebView.EvaluateJavaScriptAsync("app.fitToFeaturesExtent()");
    }
}