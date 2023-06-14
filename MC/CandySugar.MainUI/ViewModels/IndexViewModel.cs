using CandySugar.Com.Controls;
using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.GenericAction;
using CandySugar.Com.Library.OptionModel;
using CandySugar.Com.Pages.ViewModels;
using CandySugar.Com.Pages.Views;
using XExten.Advance.LinqFramework;

namespace CandySugar.MainUI.ViewModels
{
    public class IndexViewModel : BaseVMModule
    {
        private BaseVMService BaseServices;
        private CandyUIPage IndexView;
        public IndexViewModel(BaseVMService baseServices) : base(baseServices)
        {
            BaseServices = baseServices;
            this.Content = new Home
            {
                BindingContext = new HomeViewModel(baseServices)
            };
            GenericDelegate.SearchAction = new(QueryMethod);
        }

        #region Override
        public async override void OnAppearing()
        {
            await Permissions.RequestAsync<Permissions.StorageWrite>();
            await Permissions.RequestAsync<Permissions.StorageRead>();
        }
        #endregion

        #region Property
        private View _Content;
        public View Content { get => _Content; set => SetProperty(ref _Content, value); }
        #endregion

        #region Command
        public DelegateCommand<string> NavCommand => new(NavMethod);
        public DelegateCommand<CandyUIPage> NavShowCommand => new(ShowMethod);
        #endregion

        #region Method
        private void ShowMethod(CandyUIPage page)
        {
            IndexView = page;
            ((LeftPage)IndexView.Attachments.First()).IsPresented = true;
        }
        private void NavMethod(string param)
        {
            NavCommonMethod(param.AsInt());
        }
        private void QueryMethod(SearchOptionModel param)
        {
            NavCommonMethod(param.Hint, param.Description);
        }
        private  void NavCommonMethod(int param, string key = "")
        {
            Application.Current.Dispatcher.DispatchAsync(() =>
            {
                if (param == 1)
                {
                    Content = new Rifan
                    {
                        BindingContext = new RifanViewModel(key, this.BaseServices)
                    };
                }
                ((LeftPage)IndexView.Attachments.First()).IsPresented = false;
            });
        }
        #endregion
    }
}
