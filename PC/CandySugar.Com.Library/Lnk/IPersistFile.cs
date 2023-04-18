using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Lnk
{
    [ComImport()]
    [Guid("0000010B-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile
    {
        [PreserveSig()]
        int GetClassID(out Guid pClassID);

        [PreserveSig()]
        int IsDirty();

        [PreserveSig()]
        int Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

        [PreserveSig()]
        int Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig()]
        int SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig()]
        int GetCurFile([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath);
    }
}
