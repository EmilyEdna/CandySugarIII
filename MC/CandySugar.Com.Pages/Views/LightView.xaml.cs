using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class LightView : ContentPage
{
	public LightView()
	{
		InitializeComponent();
		this.BindingContext = new LightViewModel();
	}
}