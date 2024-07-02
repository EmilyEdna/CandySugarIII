using CommunityToolkit.Mvvm.ComponentModel;

namespace CandySugar.LightNovel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
                ComponentControl= Module.IocModule.Resolve<IndexView>();
        }


        #region 属性
        [ObservableProperty]
        private Control _ComponentControl;
        #endregion
    }
}
