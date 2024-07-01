using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Interop;

namespace CandySugar.Com.Library.Cursors
{
    public class CursorExten
    {
      
        /// <summary>
        /// 根据 本地文件路径 创建鼠标图标
        /// </summary>
        /// <param name="filePath">鼠标图像全路径</param>
        /// <param name="xHotSpot">焦点在图片中的 X轴 坐标(相对于左上角)</param>
        /// <param name="yHotSpot">焦点在图片中的 Y轴 坐标(相对于左上角)</param>
        /// <returns>错误则返回null</returns>
        public static Cursor CreateCursor(string filePath, int xHotSpot = 0, int yHotSpot = 0)
        {
            Cursor ret = null;

            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) || !File.Exists(filePath))
            {
                return ret;
            }

            //首先尝试通过默认方法创建
            if (filePath.EndsWith(".ani") || filePath.EndsWith(".cur"))
            {
                try
                {
                    ret = new Cursor(filePath);
                }
                catch (Exception)
                {
                    ret = null;
                }
            }

            //如果文件不是正确的.ani或.cur文件，则尝试通过BitMap创建
            if (ret == null)
            {

                Bitmap bmp = null;
                try
                {
                    bmp = Bitmap.FromFile(filePath) as Bitmap;
                    if (bmp != null)
                    {
                        ret = CreateCursor(bmp, xHotSpot, yHotSpot);
                    }
                }
                catch (Exception)
                {
                    ret = null;
                }
            }

            return ret;
        }

        /// <summary>
        /// 根据 Bitmap 创建自定义鼠标
        /// </summary>
        /// <param name="bm">鼠标图像</param>
        /// <param name="xHotSpot">焦点在图片中的 X轴 坐标(相对于左上角)</param>
        /// <param name="yHotSpot">焦点在图片中的 Y轴 坐标(相对于左上角)</param>
        /// <returns>错误则返回null</returns>
        public static Cursor CreateCursor(Bitmap bm, int xHotSpot = 0, int yHotSpot = 0)
        {
           return InternalCreateCursor(bm, xHotSpot, yHotSpot); ;
        }

        /// <summary>
        /// 创建鼠标（本方法不允许public，避免内存泄漏）
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="xHotSpot"></param>
        /// <param name="yHotSpot"></param>
        /// <returns></returns>
        protected static Cursor InternalCreateCursor(Bitmap bitmap, int xHotSpot, int yHotSpot)
        {
            var iconInfo = new IconInfo();
            CursorManager.GetIconInfo(bitmap.GetHicon(), ref iconInfo);

            iconInfo.xHotspot = xHotSpot;//焦点x轴坐标
            iconInfo.yHotspot = yHotSpot;//焦点y轴坐标
            iconInfo.fIcon = false;//设置鼠标

            SafeIconHandle cursorHandle = CursorManager.CreateIconIndirect(ref iconInfo);
            return CursorInteropHelper.Create(cursorHandle);
        }
    }
}
