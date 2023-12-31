using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class AxgleView : ContentPage
{
	public AxgleView()
	{
		InitializeComponent();
        this.BindingContext = new AxgleViewModel();
    }
}