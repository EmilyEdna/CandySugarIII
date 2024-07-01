using CandySugar.Rifan.View;

namespace CandySugar.Rifan.ViewModels
{
    public class MainViewModel: PropertyChangedBase
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
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
