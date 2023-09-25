using CandySugar.Com.Library.BaseViewModel;

namespace CandySugar.Com.Pages.ViewModels.ComicViewModels
{
    public class ComicWatchViewModel : BaseVMModule
    {
        public ComicWatchViewModel(BaseVMService baseServices) : base(baseServices)
        {
            Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density)-250;
            Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density)-25;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Index = parameters.GetValue<int>("Index");
            Views = parameters.GetValue<List<string>>("Data");
            Current = Views[Index];
        }

        #region Field
        private List<string> Views;
        private int Index;
        #endregion

        #region  Property
        private string _Current;
        public string Current
        {
            get => _Current;
            set => SetProperty(ref _Current, value);
        }

        private double _Height;
        public double Height
        {
            get => _Height;
            set => SetProperty(ref _Height, value);
        }

        private double _Width;
        public double Width
        {
            get => _Width;
            set => SetProperty(ref _Width, value);
        }
        #endregion

        #region Command
        public DelegateCommand NextCommand => new(() => {
            if (Index < Views.Count)
            {
                Index += 1;
                Current = Views[Index];
            }
        });
        public DelegateCommand PreCommand => new(() => {
            if (Index >0)
            {
                Index -= 1;
                Current = Views[Index];
            }
        });
        #endregion
    }
}
