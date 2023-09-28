using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CandySugar.Com.Pages.ChildViewModels.Comics
{
    public class CatalogViewModel : ObservableObject
    {
        public RelayCommand Next => new(NextF);
        async void NextF()
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VisitView)]);
        }
    }
}
