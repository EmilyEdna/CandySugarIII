using CandySugar.Com.Library.ReadFile;
using CandySugar.Com.Options.ComponentObject;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Documents;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library
{
    public class ComponentBinding
    {

        private static ComponentObjectModelGroup _ComponentObjectModelGroups;
        /// <summary>
        /// 组件信息
        /// </summary>
        public static ComponentObjectModelGroup ComponentObjectModelGroups
        {
            get
            {
                if (_ComponentObjectModelGroups != null) return _ComponentObjectModelGroups;
                else
                {
                    ComponentObjectModelGroup Model = new ComponentObjectModelGroup();
                    JsonReader.Configuration.Bind("ComponentInfos", Model);
                    _ComponentObjectModelGroups = Model;
                    return _ComponentObjectModelGroups;
                }
            }
        }

        private static List<FunctionObjectModel> _FunctionObjectModels;
        /// <summary>
        /// 系统功能
        /// </summary>
        public static List<FunctionObjectModel> FunctionObjectModels
        {
            get
            {
                if (_FunctionObjectModels != null) return _FunctionObjectModels;
                else
                {
                    List<FunctionObjectModel> Model = new List<FunctionObjectModel>();
                    JsonReader.Configuration.Bind("System", Model);
                    _FunctionObjectModels = Model;
                    return _FunctionObjectModels;
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
