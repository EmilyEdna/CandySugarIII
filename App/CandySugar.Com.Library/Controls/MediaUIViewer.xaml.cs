using System.Text.RegularExpressions;
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

    private int CountIndex = 0;
    private void ButtonEvent(object sender, EventArgs e)
    {
        object CommandParameter = null;
        if (sender is ImageButton)
            CommandParameter = ((ImageButton)sender).CommandParameter;
        else
            CommandParameter = ((Button)sender).CommandParameter;
        Media.HeightRequest = 350;
        var index = CommandParameter.ToString().AsInt();
        if (index == 1)
        {
            if (!Media.IsPlaying) return;
            
            CountIndex += 1;
            if (CountIndex==1)
                Rate.Text = "X2";
            if(CountIndex==2)
                Rate.Text = "X4";
            if (CountIndex == 3)
            {
                CountIndex = 0;
                Rate.Text = "X1";
            }
            Media.SetRate(float.Parse(Regex.Match(Rate.Text,"\\d+").Value));
        }
		if (index == 2)
		{
			Play.IsVisible = false;
			Pause.IsVisible = true;
			Media.Play();
        }
        if (index == 3)
        {
            Play.IsVisible = true;
            Pause.IsVisible = false;
            Media.Pause();
		}
        if (index == 4)
        {
            Play.IsVisible = true;
            Pause.IsVisible = false;
            Media.Reload();
        }
    }

    private void ProgressChanged(object sender, EventArgs e)
    {
        Media.CurrentChange((sender as Slider).Value);
    }

    public void Dispose()
    {
        Media.Dispose();
    }

}