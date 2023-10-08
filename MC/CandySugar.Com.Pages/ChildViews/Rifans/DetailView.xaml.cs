using CandySugar.Com.Pages.ChildViewModels.Rifans;

namespace CandySugar.Com.Pages.ChildViews.Rifans;

public partial class DetailView : ContentPage
{
	public DetailView()
	{
		InitializeComponent();
		this.BindingContext = new DetailViewModel();
	}
}