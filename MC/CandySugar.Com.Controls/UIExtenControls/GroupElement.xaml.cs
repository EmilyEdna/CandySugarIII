using CandySugar.Com.Library.Extends;
using CandySugar.Com.Library.GenericAction;
using CandySugar.Com.Library.OptionModel;
using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Platform;

namespace CandySugar.Com.Controls;

public partial class GroupElement : ContentView
{
    private int Hint = 0;
    public GroupElement()
    {
        InitializeComponent();
        BorderHandler.WidthRequest = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 50;
        Category.ItemsSource = new List<SearchOptionModel>
        {
             new SearchOptionModel{ Hint=1,Description="Rifan" },
             new SearchOptionModel{ Hint=2,Description="Comic" },
             new SearchOptionModel{ Hint=3,Description="Avgle" },
        };
        Category.ItemDisplayBinding = new Binding("Description");
    }

    private void PickerChanged(object sender, EventArgs e)
    {
         Hint = (((Picker)sender).SelectedItem as SearchOptionModel).Hint;
    }

    private async void EntryCompleted(object sender, EventArgs e)
    {
        if (Hint == 0)
        {
            AppExtend.NoSelectCategory.Info();
            return;
        }
        var entry = ((Entry)sender);
        await entry.HideKeyboardAsync(CancellationToken.None);
        GenericDelegate.SearchAction?.Invoke(new SearchOptionModel { Hint = Hint, Description = entry.Text });
    }
}