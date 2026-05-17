using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.OptionModel
{
    public class PlayInfo: BindableBase
    {
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }
        private string _Clarity;
        public string Clarity
        {
            get => _Clarity;
            set => SetProperty(ref _Clarity, value);
        }
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetProperty(ref _Route, value);
        }
    }
}
