using CandySugar.Com.Pages.ChildViewModels.Axgles;

namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class LinkView : ContentPage
{
	public LinkView()
	{
		InitializeComponent();
		this.BindingContext = new LinkViewModel();

    }
}