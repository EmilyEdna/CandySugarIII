﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CandySugar.Com.Library.BitConvert
{
    public class BitmapHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// BitmapSource 转 Bitmap
        /// </summary>
        /// <param name="bi"></param>
        /// <returns>错误则返回null</returns>
        public static Bitmap BitmapSource2Bitmap(BitmapSource bitmapSource)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BmpBitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }
        /// <summary>
        /// BitmapImage 转 Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }
        /// <summary>
        /// bytes转图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapSource Bytes2Image(byte[] bytes, int width = 160, int height = 240)
        {
            Bitmap bmp = Image.FromStream(new MemoryStream(bytes)) as Bitmap;
            var ptr = bmp.GetHbitmap();
            var source = Imaging.CreateBitmapSourceFromHBitmap(
                  ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));
            source.Freeze();
            bmp.Dispose();
            DeleteObject(ptr);
            return source;
        }
    }
}
