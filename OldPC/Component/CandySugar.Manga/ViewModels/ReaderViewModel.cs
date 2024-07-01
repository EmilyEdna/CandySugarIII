using CandySugar.Com.Library.BitConvert;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CandySugar.Manga.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {

        #region Property
        private ObservableCollection<string> _Picture;
        public ObservableCollection<string> Picture
        {
            get => _Picture;
            set => SetAndNotify(ref _Picture, value);
        }
        #endregion

        #region Command
        public void BackCommand()
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl,
                ControlType = 1
            });
        }
        #endregion

        #region Method
        public void SetImages()
        {
            var imgBytes = ModuleEnv.GlobalTempParam as List<byte[]>;
            Picture = new(imgBytes.Select(Convert.ToBase64String));
        }
        #endregion
    }
}
