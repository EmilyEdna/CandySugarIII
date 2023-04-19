using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.DLLoader;
using CandySugar.Com.Library.DownQueue;
using CandySugar.Com.Library.ReadFile;
using CandySugar.Com.Options.ComponentObject;
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
            Com.Library.Lnk.Shortcut.Instance.CreateLnk("Candy");
            #endif
            //日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(CommonHelper.LogPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            JsonReader.JsonRead(CommonHelper.OptionPath, CommonHelper.OptionFile);
            AssemblyLoader Loader = new(CommonHelper.AppPath);
            ComponentBinding.ComponentObjectModels.ForEach(Dll =>
            {
                Loader.Load(Dll.Plugin, Dll.Bootstrapper, Dll.Ioc, Dll.Description);
            });
            HttpEvent.HttpActionEvent = new Action<HttpClient, Exception>((client, ex) =>
            {
                Log.Logger.Error(ex, "HTTP全局请求异常捕获");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new ScreenNotifyView($"HTTP网络内部异常，请看日志!").Show();
                });
            });
            HttpEvent.RestActionEvent = new Action<RestClient, Exception>((client, ex) =>
            {
                Log.Logger.Error(ex, "REST全局请求异常捕获");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new ScreenNotifyView($"REST网络内部异常，请看日志!").Show();
                });
            });
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<OptionViewModel>().ToSelf();
            builder.Bind<AboutViewModel>().ToSelf();
            AssemblyLoader.Dll.ForEach(item =>
            {
                if (item.IocModule != null)
                    Activator.CreateInstance(item.IocModule);
            });
        }

        /// <summary>
        /// 初始化系统相关参数配置
        /// </summary>
        protected override void Configure()
        {
            DownloadRequest.DownByQueue();
            base.Configure();
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
