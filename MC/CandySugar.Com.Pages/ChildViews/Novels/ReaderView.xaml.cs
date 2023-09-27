using CandySugar.Com.Pages.ChildViewModels.Novels;

namespace CandySugar.Com.Pages.ChildViews.Novels;

public partial class ReaderView : ContentPage
{
    public ReaderView()
    {
        InitializeComponent();
        this.BindingContext = new ReaderViewModel();
    }
}