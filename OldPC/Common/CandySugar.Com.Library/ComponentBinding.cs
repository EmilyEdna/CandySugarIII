using CandySugar.Com.Library.ReadFile;
using CandySugar.Com.Options.ComponentObject;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library
{
    public class ComponentBinding
    {

        private static List<ComponentObjectModel> _ComponentObjectModels;
        /// <summary>
        /// 组件信息
        /// </summary>
        public static List<ComponentObjectModel> ComponentObjectModels
        {
            get
            {
                if (_ComponentObjectModels != null) return _ComponentObjectModels;
                else
                {
                    List<ComponentObjectModel> Model = new();
                    JsonReader.Configuration.Bind("ComponentInfos", Model);
                    _ComponentObjectModels = Model;
                    return _ComponentObjectModels;
                }
            }
        }

        private static OptionObjectModel _OptionObjectModels;
        /// <summary>
        /// 系统配置
        /// </summary>
        public static OptionObjectModel OptionObjectModels
        {
            get
            {
                lock (locker)
                {
                    if (ForceRefresh) ForceRefreshOptionObjectModels();
                    if (_OptionObjectModels != null) return _OptionObjectModels;
                    else
                    {
                        OptionObjectModel Model = new();
                        JsonReader.Configuration.Bind("Option", Model);
                        _OptionObjectModels = Model;
                        WeakReferenceMessenger.Default.Send(Model);
                        return _OptionObjectModels;
                    }
                }
            }
        }
        private static readonly object locker = new();
        public static bool ForceRefresh { get; set; }
        /// <summary>
        /// 强制属性配置
        /// </summary>
        private static void ForceRefreshOptionObjectModels() 
        {
            Thread.Sleep(500);
            OptionObjectModel Model = new();
            JsonReader.Configuration.Bind("Option", Model);
            if (Model.BackgroudLocation.IsNullOrEmpty())
                ForceRefreshOptionObjectModels();
            _OptionObjectModels = Model;
            WeakReferenceMessenger.Default.Send(Model);
            ForceRefresh = false;
        }
    }
}
