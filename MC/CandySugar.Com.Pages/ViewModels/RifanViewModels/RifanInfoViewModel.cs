using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.MsgModel;
using CandySugar.Com.Library.OptionModel;
using CommunityToolkit.Mvvm.Messaging;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Pages.ViewModels.RifanViewModels
{
    public class RifanInfoViewModel : BaseVMModule
    {
        public RifanInfoViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            WeakReferenceMessenger.Default.Register<MessageModel>(this, (recep, handle) => {
                Info = new ObservableCollection<PlayInfo>(handle.PlayInfos);
                Link = new ObservableCollection<RifanSearchModel>(handle.RifanModels);
            });
        }

        #region Property
        private ObservableCollection<PlayInfo> _Info;
        public ObservableCollection<PlayInfo> Info
        {
            get => _Info;
            set=>SetProperty(ref _Info, value);
        }

        private ObservableCollection<RifanSearchModel> _Link;
        public ObservableCollection<RifanSearchModel> Link
        {
            get => _Link;
            set=>SetProperty(ref  this._Link, value);
        }
        #endregion
    }
}
