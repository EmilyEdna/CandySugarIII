using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class AvgleView : ContentPage
{
    private AvgleViewModel VM;

    public AvgleView()
	{
		InitializeComponent();
        VM = new AvgleViewModel();
        this.BindingContext = VM;
    }

    private void SelectEvent(object sender, EventArgs e)
    {
        VM.TagChanged((sender as Picker).SelectedItem.ToString());
    }
}