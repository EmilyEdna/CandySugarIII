using CandySugar.Com.Pages.ChildViewModels.Lights;

namespace CandySugar.Com.Pages.ChildViews.Lights;

public partial class ChaptersView : ContentPage
{
	public ChaptersView()
	{
		InitializeComponent();
		this.BindingContext = new ChaptersViewModel();
    }
}