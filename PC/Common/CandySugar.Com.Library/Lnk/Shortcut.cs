using System;
using System.IO;

namespace CandySugar.Com.Library.Lnk
{
    public class Shortcut
    {
        public static Shortcut Instance => new();
        /// <summary>
        /// 创建桌面图标
        /// </summary>
        public void CreateLnk(string name)
        {
            IShellLink link = (IShellLink)new ShellLink();
            //快捷方式的描述
            link.SetDescription("甜糖");
            //设置快捷方式的目标所在的位置(源程序完整路径)
            link.SetPath(Path.Combine(CommonHelper.AppPath, $"{name}Sugar.exe"));
            IPersistFile file = (IPersistFile)link;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");
            if (!File.Exists(path))
                file.Save(path, false);
        }
    }

}