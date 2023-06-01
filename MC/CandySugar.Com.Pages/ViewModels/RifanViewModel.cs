using CandySugar.Com.Library.BaseViewModel;

namespace CandySugar.Com.Pages.ViewModels
{
    public class RifanViewModel : BaseVMModule
    {
        public RifanViewModel(string QueryKey, BaseVMService baseServices) : base(baseServices)
        {
        }

        #region Method
        private void ChangeMethod(int param)
        { 
        
        }
        #endregion

        #region Command
        public DelegateCommand<int> ChangeCommand => new(ChangeMethod);
        #endregion
    }
}
