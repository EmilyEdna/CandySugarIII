namespace CandySugar.Novel.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
        }
        public MainViewModel(object data) 
        {
            Module.Param = data;
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
