using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Controls
{
    public interface IPageAttachment : IView
    {
        void OnAttached(CandyUIPage attachedPage);
        AttachmentLocation AttachmentPosition { get; }
    }
}
