using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class JAgleView : ContentPage
{
    private JAgleViewModel VM;
    public JAgleView()
	{
		InitializeComponent();
        VM = new JAgleViewModel();
        this.BindingContext = VM;
    }
    private void SelectEvent(object sender, EventArgs e)
    {
        VM.TagChanged((sender as Picker).SelectedItem.ToString());
    }
}