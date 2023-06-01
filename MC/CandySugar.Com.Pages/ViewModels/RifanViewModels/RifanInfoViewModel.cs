using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.MsgModel;
using CandySugar.Com.Library.OptionModel;
using CommunityToolkit.Mvvm.Messaging;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using System;
using System.Collections.Generic;
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


            });
        }
    }
}
