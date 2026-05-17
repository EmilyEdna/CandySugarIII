namespace CandySugar.Axgle.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel(string input)
        {
            PlatformEnum Platform = Enum.Parse<PlatformEnum>(input);
            Control View = null;
            if (Platform == PlatformEnum.Skb) View = Module.IocModule.Resolve<Index1View>();
            if (Platform == PlatformEnum.Jav) View = Module.IocModule.Resolve<Index2View>();
            if (Platform == PlatformEnum.A24) View = Module.IocModule.Resolve<Index3View>();
            if (Platform == PlatformEnum.JAXX) View = Module.IocModule.Resolve<Index4View>();
            ComponentControl = View;
        }
        #region Property
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion
    }
}
