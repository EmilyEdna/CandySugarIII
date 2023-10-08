using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class AnimeView : ContentPage
{
	public AnimeView()
	{
		InitializeComponent();
		this.BindingContext = new AnimeViewModel();
	}
}