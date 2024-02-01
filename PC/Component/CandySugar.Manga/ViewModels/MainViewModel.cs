namespace CandySugar.Manga.ViewModels
{
    public class MainViewModel: PropertyChangedBase
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (notify.NotifyType == NotifyType.ChangeControl)
                    {
                        ModuleEnv.GlobalTempParam = notify.ControlParam;
                        if (notify.ControlType == 1) ComponentControl = Module.IocModule.Resolve<IndexView>();
                        if (notify.ControlType == 2)
                        {
                            var Reader = Module.IocModule.Resolve<ReaderView>();
                            Reader.Height = ComponentControl.Height;
                            Reader.Width = ComponentControl.Width;
                            ComponentControl = Reader;
                        }
                    }
                });
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
