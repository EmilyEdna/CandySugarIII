namespace CandySugar.Axgle.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = new ObservableCollection<AnonymousTab>
            {
                new AnonymousTab{ Title="SKB",Value = Module.IocModule.Resolve<Index1View>() },
                new AnonymousTab{ Title="JAV",Value = Module.IocModule.Resolve<Index2View>() },
                new AnonymousTab{ Title="H24",Value = Module.IocModule.Resolve<Index3View>() },
                new AnonymousTab{ Title="JAX",Value = Module.IocModule.Resolve<Index4View>() },
                new AnonymousTab{ Title="收藏",Value = Module.IocModule.Resolve<Index5View>() },
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
            if (GlobalParam.WindowState == WindowState.Maximized) {
                foreach (var item in ComponentControl)
                {
                    item.Width = GlobalParam.MAXWidth+152d;
                }
            }
            else
            {
                foreach (var item in ComponentControl)
                {
                    item.Width = GlobalParam.MAXWidth-60;
                }
            }
        }
        #endregion

    }
}
