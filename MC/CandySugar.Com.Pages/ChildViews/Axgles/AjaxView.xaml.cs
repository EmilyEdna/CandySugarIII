using CandySugar.Com.Pages.ChildViewModels.Axgles;
using CommunityToolkit.Mvvm.Messaging;

namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class AjaxView : ContentPage
{
	public AjaxView()
	{
		InitializeComponent();
		this.BindingContext = new AjaxViewModel();
		WeakReferenceMessenger.Default.Register<AjaxViewModel>(this, (o, v) =>
		{
			this.Player.Reload();
        });
	}
}