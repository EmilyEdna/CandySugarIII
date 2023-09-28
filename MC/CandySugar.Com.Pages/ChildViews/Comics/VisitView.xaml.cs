using CandySugar.Com.Pages.ChildViewModels.Comics;

namespace CandySugar.Com.Pages.ChildViews.Comics;

public partial class VisitView : ContentPage
{
	public VisitView()
	{
		InitializeComponent();
		this.BindingContext = new VisitViewModel();
	}
}