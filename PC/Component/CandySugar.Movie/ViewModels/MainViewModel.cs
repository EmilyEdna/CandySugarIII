namespace CandySugar.Movie.ViewModels
{
    public partial class MainViewModel: ObservableObject
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
