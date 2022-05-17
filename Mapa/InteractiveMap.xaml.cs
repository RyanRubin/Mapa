namespace Mapa;

public partial class InteractiveMap : ContentView
{
	public InteractiveMap()
	{
		InitializeComponent();

		mapWebView.Source = "https://www.google.com/";
	}
}