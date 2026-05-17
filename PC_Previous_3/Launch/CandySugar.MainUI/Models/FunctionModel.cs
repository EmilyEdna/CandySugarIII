using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CandySugar.MainUI.Models
{
    public partial class FunctionModel:ObservableObject
    {
        [ObservableProperty]
        string _Title;
        [ObservableProperty]
        Control _CandyControl;
    }
}
