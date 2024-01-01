using CandySugar.Com.Pages.ChildViewModels.Axgles;

namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class AjaxView : ContentPage
{
	public AjaxView()
	{
		InitializeComponent();
		this.BindingContext = new AjaxViewModel();
	}
}