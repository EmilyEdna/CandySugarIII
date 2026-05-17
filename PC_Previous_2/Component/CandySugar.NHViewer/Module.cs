namespace CandySugar.NHViewer
{
    public class Module
    {
        public static object Param { get; set; }

        public static Module IocModule { get; set; }

        public ProxyObjectModel Proxy => GlobalProxy.Instance.Proxy();

        public Module()
        {
            IocModule = this;
            IocDependency.Register(typeof(HIndexView));
            IocDependency.Register(typeof(HIndexViewModel));
            IocDependency.Register(typeof(NIndexView));
            IocDependency.Register(typeof(NIndexViewModel));
            IocDependency.Register(typeof(ReaderView));
            IocDependency.Register(typeof(ReaderViewModel), 1);
        }

        public T Resolve<T>() where T : UserControl
        {
            var Ctrl = (UserControl)IocDependency.Resolve(typeof(T));
            var VM = this.GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{typeof(T).Name}Model");
            Ctrl.DataContext = IocDependency.Resolve(VM);
            return (T)Ctrl;
        }
    }
}
