using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Controls
{
    public interface IViewAttachment : IView
    {
        void OnAttached(CandyUIView attachedView);
        AttachmentLocation AttachmentPosition { get; }
    }
}
