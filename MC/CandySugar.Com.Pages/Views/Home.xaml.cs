using CandySugar.Com.Pages.ViewModels;
using Microsoft.Maui.Platform;
using XExten.Advance.ThreadFramework;

namespace CandySugar.Com.Pages.Views;

public partial class Home : ContentView
{

    private bool Rotated = false;
    private List<ImageButton> ImageBtn = new List<ImageButton>();
    private List<string> ItemSource = new List<string>();

    public Home()
    {
        InitializeComponent();
        this.Loaded += delegate
        {
            ItemSource.Add(Library.FontIcon.PepperHot);
            ItemSource.Add(Library.FontIcon.Palette);
            ItemSource.Add(Library.FontIcon.Spoon);
            int Index = 0;
            foreach (var item in ItemSource)
            {
                var btn = new ImageButton
                {
                    CornerRadius = 30,
                    HeightRequest = 90,
                    WidthRequest = 90,
                    Background = new SolidColorBrush(Color.FromHex("#20B2AA")),
                    CommandParameter = Index,

                    Margin = new Thickness(16),
                    Padding = new Thickness(16),
                    Source = new FontImageSource
                    {
                        FontFamily = "Thin",
                        Glyph = item.ToString()
                    }
                };
                btn.Clicked += (sender, e) =>
                {
                    var btn = (sender as ImageButton);
                    (this.BindingContext as HomeViewModel).SetContent((int)btn.CommandParameter+1);
                };
                this.FloatContainer.Add(btn);
                Index++;
            }
        };
    }


    private void ClickEvent(object sender, EventArgs e)
    {
        var btn = (sender as ImageButton);
        btn.RotateTo(Rotated ? 0 : -45);

        FloatContainer.Animate("Grid", tk => !Rotated ? true : false, ntk => FloatContainer.IsVisible = ntk, finished: (_, _) =>
        {
            Rotated = !Rotated;
            var btn = (FloatContainer.Children.First() as ImageButton);
            ImageBtn.Add(btn);
            ExcutorAnime(btn);
            ThreadFactory.Instance.StartWithRestart(() =>
            {
                if (FloatContainer.Children.Count == ImageBtn.Count)
                    ImageBtn.Clear();
            }, "Handle", true);
        });
    }

    private void ExcutorAnime(ImageButton btn)
    {
        btn.Animate($"{btn.CommandParameter}", tk =>
        {
            if (tk == 1)
            {
                var buttom = Rotated ? -45+(int)btn.CommandParameter*120 : 16;
                return new Thickness(16, 16, 16, buttom);
            }
            else
                return new Thickness(16);
        }, ntk => btn.Margin = ntk, finished: (_, _) =>
        {
            if (ImageBtn.Count > 0)
            {
                var bt = FloatContainer.Children.OfType<ImageButton>().Where(t => !ImageBtn.Select(t => t.CommandParameter.ToString()).Contains(t.CommandParameter.ToString())).FirstOrDefault();
                if (bt != null)
                {
                    if (!ImageBtn.Any(t => t.CommandParameter == bt.CommandParameter))
                    {
                        ImageBtn.Add(bt);
                        ExcutorAnime(bt);
                    }
                }
            }
        });
    }
}