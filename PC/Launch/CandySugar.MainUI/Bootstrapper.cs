using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Data;
using CandySugar.Com.Library;
using CandySugar.Com.Library.DLLoader;
using CandySugar.Com.Library.ReadFile;
using CandySugar.Com.Options;
using CandySugar.HostServer;
using CandySugar.MainUI.ViewModels;
using RestSharp;
using Serilog;
using Stylet;
using StyletIoC;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using XExten.Advance;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework;
using XExten.Advance.StaticFramework;

namespace CandySugar.MainUI
{
    public class Bootstrapper : Bootstrapper<IndexViewModel>
    {
        /// <summary>
        /// 程序启动
        /// </summary>
        protected override void OnStart()
        {
#if RELEASE
            //创建桌面快捷方式
            Com.Library.Lnk.Shortcut.Instance.CreateLnk("Candy");
#endif
            //日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(CommonHelper.LogPath, rollingInterval: RollingInterval.Day)
                .CreateLogger().AddSdkLogger().AddEmailLogger();
            //代理配置
            GlobalProxy.Instance.ChangeUseProxy();
            //读取本地配置
            JsonReader.JsonRead(CommonHelper.OptionPath, CommonHelper.OptionFile);
            //读取本地插件配置
            AssemblyLoader Loader = new(CommonHelper.AppPath);
            ComponentBinding.ComponentObjectModelGroups.Normal?.ForEach(Loader.Loads);
            ComponentBinding.ComponentObjectModelGroups.Vip?.ForEach(Loader.Loads);
            //注册请求框架
            NetFactoryExtension.RegisterNetFramework();
            //初始化粘贴板
            ClipboardUtil.InitClipBoard();
            //配置请求框架全局异常
            HttpEvent.HttpActionEvent = new Action<HttpClient, Exception>((client, ex) =>
            {
                Log.Logger.Error(ex, "HTTP全局请求异常捕获");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new CandyNotifyControl($"HTTP网络内部异常，请看日志!").Show();
                });
            });
            HttpEvent.RestActionEvent = new Action<RestClient, Exception>((client, ex) =>
            {
                Log.Logger.Error(ex, "REST全局请求异常捕获");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new CandyNotifyControl($"REST网络内部异常，请看日志!").Show();
                });
            });
            //防止多开程序
            if (SyncStatic.MultiOpenCheck())
                new CandyNotifyControl("已经有一个实例在运行中").ShowDialog();
        }

        /// <summary>
        /// 注入模型
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<OptionViewModel>().ToSelf();
            AssemblyLoader.Dll.ForEach(item =>
            {
                if (item.IocModule != null)
                    Activator.CreateInstance(item.IocModule);
            });
            Module.Services.ForDicEach((key, value) => IocDependency.Register(value, key));
        }

        /// <summary>
        /// 初始化系统相关参数配置
        /// </summary>
        protected override void Configure()
        {
            base.Configure();
            WebHost.StartWeb();
        }

        /// <summary>
        /// 初始化VM
        /// </summary>
        protected override void Launch()
        {
            base.Launch();
        }

        /// <summary>
        /// 加载首页VM
        /// </summary>
        /// <param name="rootViewModel"></param>
        protected override void DisplayRootView(object rootViewModel)
        {
            base.DisplayRootView(rootViewModel);
        }

        /// <summary>
        ///VM加载完毕
        /// </summary>
        protected override void OnLaunch()
        {
            base.OnLaunch();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        /// <summary>
        /// 全局异常捕获
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.Error(e.Exception.InnerException ?? e.Exception, "");
            GC.Collect();
        }
    }
}
