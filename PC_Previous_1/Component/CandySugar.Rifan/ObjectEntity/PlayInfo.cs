using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Rifan.ObjectEntity
{
    public class PlayInfo: PropertyChangedBase
    {
        private string _Name;
        public string Name
        {
            get => _Name; 
            set => SetAndNotify(ref _Name, value);
        }
        private string _Clarity;
        public string Clarity
        {
            get => _Clarity;
            set=>SetAndNotify(ref _Clarity, value);
        }
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
    }
}
