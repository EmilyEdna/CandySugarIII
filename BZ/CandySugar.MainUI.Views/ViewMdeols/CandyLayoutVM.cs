using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
namespace CandySugar.MainUI.Views.ViewMdeols
{
    public class CandyLayoutVM : LayoutComponentBase
    {
        [Inject]
        public IPopupService _PopupService { get; set; }
        protected override void OnInitialized()
        {
            Module.Handle = new(Ex => _PopupService.EnqueueSnackbarAsync(new SnackbarOptions(Ex.Message, AlertTypes.Error)));
        }

        protected StringNumber SelectIndex = 0;

        protected string Color
        {
            get
            {
                if (SelectIndex == 0) return "#B79CFF";
                else if (SelectIndex == 1) return "#FF4441";
                else if (SelectIndex == 2) return "#0089AD";
                else if (SelectIndex == 3) return "#FF8080";
                else return "#000000";
            }
        }
    }
}
