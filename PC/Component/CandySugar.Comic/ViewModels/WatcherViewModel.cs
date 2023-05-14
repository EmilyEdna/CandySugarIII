

namespace CandySugar.Comic.ViewModels
{
    public class WatcherViewModel : PropertyChangedBase
    {
        public WatcherViewModel()
        {
            Views = new ObservableCollection<WatchInfo>();
            if (ModuleEnv.GlobalTempParam != null && ModuleEnv.GlobalTempParam is ObservableCollection<string>)
            {
                var Dat = (ObservableCollection<string>)ModuleEnv.GlobalTempParam;

                for (int index = 0; index < Dat.Count; index++)
                {
                    Views.Add(new WatchInfo
                    {
                        Index = index,
                        Route = Dat[index]
                    });
                }
            }
            Current = Views.FirstOrDefault();
        }

        #region Property
        private ObservableCollection<WatchInfo> _Views;
        public ObservableCollection<WatchInfo> Views
        {
            get => _Views;
            set => SetAndNotify(ref _Views, value);
        }
        private WatchInfo _Current;
        public WatchInfo Current
        {
            get => _Current;
            set => SetAndNotify(ref _Current, value);
        }
        #endregion

        #region Command
        public RelayCommand PreviousCommand => new(() => {

            if (Current.Index - 1 < 0) return;
            Current = Views[Current.Index - 1];
        });
        public RelayCommand NextCommand => new(() =>
        {
            if (Current.Index + 1 > Views.Count) return;
            Current = Views[Current.Index + 1];
        });

        public RelayCommand BackCommand => new(() =>
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl
            });
        });
        #endregion
    }
}
