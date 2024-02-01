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
        public ReaderViewModel()
        {
            var imgBytes = ModuleEnv.GlobalTempParam as List<byte[]>;
            imgBytes.ForEach(item =>
            {
                Picture.Add(BitmapHelper.Bytes2Image(item, (int)(Width * .9), (int)(Height * .9)));
            });
        }
        #region Field
        internal double Width;
        internal double Height;
        #endregion

        #region Property
        private ObservableCollection<BitmapSource> _Picture;
        public ObservableCollection<BitmapSource> Picture
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
    }
}
