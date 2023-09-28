using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Comics
{
    public partial class VisitViewModel : ObservableObject, IQueryAttributable
    {
        public VisitViewModel()
        {
            Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density) - 250;
            Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 25;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Index = query["Index"].ToString().AsInt();
            Views = new ObservableCollection<string>((List<string>)query["Param"]);
            Current = Views[Index];
        }

        #region Property
        [ObservableProperty]
        private ObservableCollection<string> _Views;
        [ObservableProperty]
        private int _Index;
        [ObservableProperty]
        private double _Height;
        [ObservableProperty]
        private double _Width;
        [ObservableProperty]
        private string _Current;
        #endregion

        #region Command
        public RelayCommand NextCommand => new(() => {
            if (Index < Views.Count)
            {
                Index += 1;
                Current = Views[Index];
            }
        });
        public RelayCommand PreCommand => new(() => {
            if (Index > 0)
            {
                Index -= 1;
                Current = Views[Index];
            }
        });
        #endregion
    }
}
