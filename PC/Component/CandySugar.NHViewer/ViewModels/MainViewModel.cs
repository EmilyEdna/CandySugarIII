using CommunityToolkit.Mvvm.ComponentModel;

namespace CandySugar.NHViewer.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<HIndexView>();
        }

        #region 属性
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion

        #region 方法
        public void NChanged(bool arg) => ComponentControl = arg ? Module.IocModule.Resolve<ReaderView>() : Module.IocModule.Resolve<IndexView>();
        public void HChanged(bool arg) => ComponentControl = arg ? Module.IocModule.Resolve<ReaderView>() : Module.IocModule.Resolve<HIndexView>();
        #endregion
    }
}
