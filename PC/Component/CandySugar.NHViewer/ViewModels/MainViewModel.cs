using CandySugar.Com.Options.Anonymous;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Utils;

namespace CandySugar.NHViewer.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<NIndexView>();
            MenuData = new() { { "HI", "1" }, { "NH", "2" } };
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 15, 20);
            else
                MarginThickness = new Thickness(0, 0, 15, 15);
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private Control _ComponentControl;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        #endregion

        #region 命令
        [RelayCommand]
        public void Active(object input) 
        {
            var param = input.ToMapest<AnonymousWater>();
            if (param.SelectName == "Hi")
                ComponentControl = Module.IocModule.Resolve<HIndexView>();
            else
                ComponentControl = Module.IocModule.Resolve<NIndexView>();
        }
        #endregion

        #region 方法
        public void NChanged(bool arg) => ComponentControl = arg ? Module.IocModule.Resolve<ReaderView>() : Module.IocModule.Resolve<NIndexView>();
        public void HChanged(bool arg) => ComponentControl = arg ? Module.IocModule.Resolve<ReaderView>() : Module.IocModule.Resolve<HIndexView>();
        #endregion
    }
}
