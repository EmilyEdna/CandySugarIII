using CandySugar.Com.Options.ComponentGeneric;
using System.Linq;
using System.Threading;
using System.Windows;
using XExten.Advance.ThreadFramework;

namespace CandySugar.Com.Library.ReadFile
{
    public class ClipboardUtil
    {
        public static void InitClipBoard()
        {
            ThreadFactory.Instance.StartWithRestart(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Clipboard.ContainsText())
                        if (Clipboard.GetText().Contains("www.bilibili.com/video"))
                            GenericDelegate.ClipboardAction?.Invoke(Clipboard.GetText());
                });
                Thread.Sleep(300);
            }, "ClipBoardCheck", true);

        }
    }
}
