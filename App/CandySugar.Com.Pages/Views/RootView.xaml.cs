using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class RootView : ContentPage
{
	public RootView()
	{
		InitializeComponent();
		this.BindingContext = new RootViewModel();
	}
}