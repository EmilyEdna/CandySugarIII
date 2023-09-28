using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class ComicView : ContentPage
{
	public ComicView()
	{
		InitializeComponent();
		this.BindingContext = new ComicViewModel();
	}
}