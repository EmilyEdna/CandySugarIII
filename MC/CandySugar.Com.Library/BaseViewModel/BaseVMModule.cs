using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.BaseViewModel
{
    public class BaseVMModule: BindableBase, IInitialize, INavigatedAware, IPageLifecycleAware
    {
        protected INavigationService Nav { get; }
        protected IPageDialogService Plog { get; }
        protected IContainerProvider Container { get; }
        protected IDialogService Dlog { get; }
        protected BaseVMModule(BaseVMService baseServices)
        {
            Container = baseServices.Container;
            Nav = baseServices.NavigationService;
            Plog = baseServices.PageDialogs;
            Dlog = baseServices.Dialogs;
            this.Activity = false;
            this.Refresh = false;
            this.Page = 1;
            OnLoad();
        }
        public virtual void Initialize(INavigationParameters parameters) { }
        public virtual void OnAppearing() { }
        public virtual void OnDisappearing() { }
        public virtual void OnLoad() { }
        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }
        public virtual void OnNavigatedTo(INavigationParameters parameters) { }

        protected void SetState()
        {
            this.Activity = false;
            this.Refresh = false;
        }
        protected void SetActivity()
        {
            Activity = true;
        }

        #region Property
        bool _Refresh;
        public bool Refresh
        {
            get { return _Refresh; }
            set { SetProperty(ref _Refresh, value); }
        }
        bool _Activity;
        public bool Activity
        {
            get { return _Activity; }
            set { SetProperty(ref _Activity, value); }
        }
        int _Page;
        public int Page
        {
            get { return _Page; }
            set { SetProperty(ref _Page, value); }
        }
        int _Total;
        public int Total
        {
            get => _Total;
            set => SetProperty(ref _Total, value);
        }
        #endregion
    }
}
