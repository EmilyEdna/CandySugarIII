using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Rifan.Model
{
    public partial class PlayInfo: ObservableObject
    {
        [ObservableProperty]
        private string _Name;
        [ObservableProperty]
        private string _Clarity;
        [ObservableProperty]
        private string _Route;

    }
}
