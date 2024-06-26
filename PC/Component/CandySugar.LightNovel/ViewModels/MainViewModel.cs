﻿namespace CandySugar.LightNovel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<ReaderView>();
        }


        #region 属性
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion

        #region 方法
        public void Changed(bool arg) => ComponentControl = arg ? Module.IocModule.Resolve<ReaderView>() : Module.IocModule.Resolve<IndexView>();
        #endregion
    }
}
