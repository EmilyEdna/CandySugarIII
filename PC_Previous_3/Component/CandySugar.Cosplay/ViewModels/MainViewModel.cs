namespace CandySugar.Cosplay.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = new ObservableCollection<AnonymousTab>
            {
                new AnonymousTab{ Title="Lab",Value = Module.IocModule.Resolve<Index1View>() },
                new AnonymousTab{ Title="Land",Value = Module.IocModule.Resolve<Index2View>() },
                new AnonymousTab{ Title="收藏",Value = Module.IocModule.Resolve<Index3View>() }
            };
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }
        #region Property
        [ObservableProperty]
        private ObservableCollection<AnonymousTab> _ComponentControl;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                foreach (var item in ComponentControl)
                {
                    item.Width = GlobalParam.MAXWidth + 152d;
                }
            }
            else
            {
                foreach (var item in ComponentControl)
                {
                    item.Width = GlobalParam.MAXWidth - 60;
                }
            }
        }
        #endregion
    }
}
