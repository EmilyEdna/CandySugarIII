using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CandySugar.Com.Pages.ViewModels
{
    public class ComicViewModel : ObservableObject
    {
        public RelayCommand Next => new(NextF);

        async void NextF()
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CatalogView)]);
        }
    }
}
