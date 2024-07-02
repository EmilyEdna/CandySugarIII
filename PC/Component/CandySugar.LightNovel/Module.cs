namespace CandySugar.LightNovel
{
    public class Module
    {
        public static object Param {  get; set; }

        public static Module IocModule { get; set; }

        public ProxyObjectModel Proxy => GlobalProxy.Instance.Proxy();

        public Module()
        {
            IocModule = this;
            IocDependency.Register(typeof(IndexView));
            IocDependency.Register(typeof(ReaderView),1);

            IocDependency.Register(typeof(IndexViewModel));
            IocDependency.Register(typeof(ReaderViewModel),1);
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
