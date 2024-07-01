using CandySugar.Com.Pages.ChildViewModels.Animes;

namespace CandySugar.Com.Pages.ChildViews.Animes;

public partial class CollectView : ContentPage
{
	public CollectView()
	{
		InitializeComponent();
        this.BindingContext = new CollectViewModel();
	}
}