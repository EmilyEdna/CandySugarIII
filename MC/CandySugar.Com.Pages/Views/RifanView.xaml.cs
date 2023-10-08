using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class RifanView : ContentPage
{
	public RifanView()
	{
		InitializeComponent();
		this.BindingContext = new RifanViewModel();
	}
}