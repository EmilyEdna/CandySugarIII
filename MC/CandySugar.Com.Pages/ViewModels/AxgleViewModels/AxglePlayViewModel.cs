using CandySugar.Com.Library.BaseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CandySugar.Com.Pages.ViewModels.AxgleViewModels
{
    public class AxglePlayViewModel : BaseVMModule
    {
        public AxglePlayViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Route = parameters.GetValue<string>("Param");
        }

        #region Property
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetProperty(ref _Route, value);
        }
        #endregion

        #region Command
        public DelegateCommand BackCommand => new(() => Nav.GoBackAsync());
        public DelegateCommand<WebView> BeginCommand => new(element =>
        {
            var x = element;
        });
        #endregion
    }
}
