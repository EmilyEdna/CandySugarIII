using CandySugar.Com.Options.ComponentObject;
using CommunityToolkit.Mvvm.Messaging;
using Sdk.Proxy;
using XExten.Advance.LinqFramework;
using XExten.Advance.LogFramework;

namespace CandySugar.Com.Options
{
    public class GlobalProxy
    {
        public static GlobalProxy Instance { get; set; } = new GlobalProxy();

        internal bool UseProxy { get; set; }
        /// <summary>
        /// 初始化代理池
        /// </summary>
        internal void InitProxyPool()
        {
            Pool.PoolLogger = new(obj => XLog.Info(obj));
            Pool.LoadProxyIP();
        }
        /// <summary>
        /// 接收使用代理通知
        /// </summary>
        public void ChangeUseProxy() => WeakReferenceMessenger.Default.Register<OptionObjectModel>(this, (recip, notify) =>
        {

            UseProxy = notify.UseProxy;
            if (UseProxy)
                InitProxyPool();
        });
        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public ProxyObjectModel Proxy() => UseProxy ? Pool.GetProxyQueeu().ToMapest<ProxyObjectModel>() : new ProxyObjectModel();
    }
}
