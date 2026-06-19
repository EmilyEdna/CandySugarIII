namespace CandySugar.Music.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region Property
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                ComponentControl.Width = GlobalParam.MAXWidth + 152d;
            }
            else
            {
                ComponentControl.Width = GlobalParam.MAXWidth - 60;
            }
        }
        #endregion
    }
}
