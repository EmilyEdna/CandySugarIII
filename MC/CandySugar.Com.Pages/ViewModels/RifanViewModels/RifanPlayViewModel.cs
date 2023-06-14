using CandySugar.Com.Library.BaseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Pages.ViewModels.RifanViewModels
{
    public class RifanPlayViewModel : BaseVMModule
    {
        public RifanPlayViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Route = parameters.GetValue<string>("Param");
        }

        #region Method
        public void GoBack()
        {
            Nav.GoBackAsync();
        }
        #endregion

        #region Property
        private string _Route;
        public string Route
        {
            get => _Route;
            set=>SetProperty(ref _Route, value);
        }
        #endregion
    }
}
