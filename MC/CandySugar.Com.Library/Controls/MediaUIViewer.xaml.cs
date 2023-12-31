using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.Controls;

public partial class MediaUIViewer : ContentView
{
	public MediaUIViewer()
	{
		InitializeComponent();
	}

	public string  Source
	{
		get { return (string )GetValue(SourceProperty); }
		set { SetValue(SourceProperty, value); }
	}

	public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(string ), typeof(MediaUIViewer),string.Empty,BindingMode.TwoWay);

    private void ButtonEvent(object sender, EventArgs e)
    {
		var btn = (sender as ImageButton);
        var index = btn.CommandParameter.ToString().AsInt();
		if (index == 1)
		{
			Play.IsVisible = false;
			Pause.IsVisible = true;
			Media.Play();
			Media.HeightRequest = 350;

        }
		else
		{
            Play.IsVisible = true;
            Pause.IsVisible = false;
            Media.Pause();
            Media.HeightRequest = 350;
		}
    }

    private void ProgressChanged(object sender, EventArgs e)
    {
        Media.CurrentChange((sender as Slider).Value);
    }
}