using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.BaseViewModel
{
    public class BaseVMService
    {
        public BaseVMService(IContainerProvider container, INavigationService navigationService, IPageDialogService pageDialogs, IDialogService dialogService, IDialogViewRegistry dialogRegistry)
        {
            NavigationService = navigationService;
            PageDialogs = pageDialogs;
            Dialogs = dialogService;
            DialogRegistry = dialogRegistry;
            Container = container;
        }
        public IContainerProvider Container { get; }
        public INavigationService NavigationService { get; }
        public IPageDialogService PageDialogs { get; }
        public IDialogService Dialogs { get; }
        public IDialogViewRegistry DialogRegistry { get; }
    }
}
