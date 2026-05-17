namespace CandySugar.Axgle.ViewModels
{
    public partial class MainViewModel : BasicObservableObject
    {
        public MainViewModel()
        {
            GenericDelegate.ChangeContentAction = new(obj => {
            
                if(!obj.ToString().IsNullOrEmpty())
                    ComponentControl= Module.IocModule.Resolve<ExpendView>();
                else
                    ComponentControl = Module.IocModule.Resolve<IndexView>();
            });
            ComponentControl = Module.IocModule.Resolve<IndexView>();
        }
        #region Property
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion
    }
}
