﻿namespace CandySugar.Rifan.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
        }
        #region Property
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion
    }
}
