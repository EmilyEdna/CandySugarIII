using Stylet;
using System.Windows;

namespace CandySugar.EntryUI.ViewModels
{
    public class AboutViewModel : PropertyChangedBase
    {
        #region Command
      
        public void CloseCommand(Window window)
        {
            window.Close();
        }
        #endregion
    }
}
