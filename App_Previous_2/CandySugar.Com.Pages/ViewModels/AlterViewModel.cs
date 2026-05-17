using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using XExten.Advance.IocFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AlterViewModel : ObservableObject
    {
        internal CollectModel CollectModel;

        #region Property
        [ObservableProperty]
        private string _Name;
        #endregion

        public RelayCommand CancelCommand
            => new(async () => await MopupService.Instance.PopAllAsync());

        public RelayCommand OkCommand => new(async () => {

            CollectModel.Name = Name;
            await IocDependency.Resolve<ICandyService>().Alter(CollectModel);
            await MopupService.Instance.PopAllAsync();
        });
    }
}
