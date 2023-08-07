using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CandySugar.MainUI.Views.ViewMdeols
{
    public class WatchVM: ComponentBase
    {

        [Parameter]
        public string? Play { get; set; }
        [Inject]
        public IJSRuntime Js { get; set; }

        protected override void OnParametersSet()
        {
            Play = Encoding.UTF8.GetString(Convert.FromBase64String(Play));
        }

        protected override void OnInitialized()
        {
            Module.HeightAction = new(async h => {
                await Js.InvokeVoidAsync("SetAuto", h-88);
            });

            base.OnInitialized();
        }
    }
}
