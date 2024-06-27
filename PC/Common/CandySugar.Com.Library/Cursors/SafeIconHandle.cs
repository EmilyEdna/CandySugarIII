using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;

namespace CandySugar.Com.Library.Cursors
{
    //[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public class SafeIconHandle: SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeIconHandle() : base(true) { }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            return CursorManager.DestroyIcon(handle);
        }
    }
}
