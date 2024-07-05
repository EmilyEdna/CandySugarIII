namespace CandySugar.WallPaper.ViewModels
{
    public partial class MainViewModel: BasicObservableObject
    {
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<IndexView>();
        }
        #region 属性
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion
    }
}
