using CandySugar.Com.Pages.ViewModels;

namespace CandySugar.Com.Pages.Views;

public partial class NovelView : ContentPage
{
	public NovelView()
	{
		InitializeComponent();
		this.BindingContext = new NovelViewModel();
	}
}