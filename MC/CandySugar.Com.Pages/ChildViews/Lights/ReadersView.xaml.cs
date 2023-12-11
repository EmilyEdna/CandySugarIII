using CandySugar.Com.Pages.ChildViewModels.Lights;

namespace CandySugar.Com.Pages.ChildViews.Lights;

public partial class ReadersView : ContentPage
{
	public ReadersView()
	{
		InitializeComponent();
        this.BindingContext = new ReadersViewModel();
    }
}