using CandySugar.Com.Pages.ChildViewModels.Novels;

namespace CandySugar.Com.Pages.ChildViews.Novels;

public partial class ChapterView : ContentPage
{
    public ChapterView()
	{
		InitializeComponent();
		this.BindingContext = new ChapterViewModel();
    }
}