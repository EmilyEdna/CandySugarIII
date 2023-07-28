using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CandySugar.Anime.ViewModels
{
    public class IndexViewModel: PropertyChangedBase
    {
        public IndexViewModel()
        {
            OnInit();
        }

        #region Field
        private int Total;
        private int PageIndex = 1;
        #endregion

        #region Property
        private ObservableCollection<CartInitElementResult> _InitResult;
        public ObservableCollection<CartInitElementResult> InitResult
        {
            get => _InitResult;
            set=>SetAndNotify(ref _InitResult, value);
        }
        #endregion

        #region  Method
        /// <summary>
        /// 初始化
        /// </summary>
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Init,
                             Init=new CartInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total=result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
            });
        }
        #endregion

        #region Command
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {

        });

        #endregion
    }
}
