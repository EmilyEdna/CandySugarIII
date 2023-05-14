namespace CandySugar.Comic.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();

            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                if (notify.NotifyType == NotifyType.ChangeControl)
                {
                    if (notify.ControlParam != null)
                    {
                        ModuleEnv.GlobalTempParam = notify.ControlParam;
                        var Reader = Module.IocModule.Resolve<WatcherView>();
                        Reader.Height = ComponentControl.Height;
                        Reader.Width = ComponentControl.Width;
                        ComponentControl = Reader;
                    }
                    else
                        ComponentControl = Module.IocModule.Resolve<IndexView>();
                }
            });

        }
        #region Property
        private Control _ComponentControl;
        public Control ComponentControl
        {
            get => _ComponentControl;
            set => SetAndNotify(ref _ComponentControl, value);
        }
        #endregion
    }
}
