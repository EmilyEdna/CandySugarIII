using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CandySugar.MainUI.Views.ViewMdeols
{
    public class WatchVM: ComponentBase
    {

        public WatchVM()
        {
            Height = 630;
            Module.Listener = new(obj => Height = obj);
        }

        public string Frame { get; set; }
        public double Height { get; set; }

        [Parameter]
        public string? Play { get; set; }

        protected override void OnParametersSet()
        {
            Play = Encoding.UTF8.GetString(Convert.FromBase64String(Play));
        }

        public void HeightChaged(double h)
        {
            Height = h;
        }
    }
}
