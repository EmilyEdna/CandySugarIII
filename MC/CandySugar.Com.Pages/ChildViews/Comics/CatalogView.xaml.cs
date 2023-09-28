using CandySugar.Com.Pages.ChildViewModels.Comics;

namespace CandySugar.Com.Pages.ChildViews.Comics;

public partial class CatalogView : ContentPage
{
	public CatalogView()
	{
		InitializeComponent();
		this.BindingContext = new CatalogViewModel();
	}
}