using CandySugar.Com.Options.ComponentGeneric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XExten.Advance.ThreadFramework;

namespace CandySugar.Com.Library.ReadFile
{
    public class ClipboardUtil
    {  
        /// <summary>
        /// 初始化粘贴板
        /// </summary>
        public static void InitClipBoard()
        {
            ThreadFactory.Instance.StartWithRestart(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Clipboard.ContainsText())
                            if (Clipboard.GetText().Contains("www.bilibili.com/video"))
                                GenericDelegate.ClipboardAction?.Invoke(Clipboard.GetText());
                    });
                    Thread.Sleep(300);
                }
            }, "ClipBoardCheck", true);

        }
    }
}
