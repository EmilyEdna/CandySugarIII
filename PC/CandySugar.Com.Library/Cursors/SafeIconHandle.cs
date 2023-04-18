using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Cursors
{
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
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
