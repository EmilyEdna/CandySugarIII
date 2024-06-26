namespace CandySugar.WallPaper
{
    public class Module
    {
        public static Module IocModule { get; set; }

        public ProxyObjectModel Proxy => GlobalProxy.Instance.Proxy();

        public Module()
        {
            IocModule = this;

            IocDependency.Register(typeof(WallhavView));
            IocDependency.Register(typeof(WallchanView));

            IocDependency.Register(typeof(WallhavViewModel));
            IocDependency.Register(typeof(WallchanViewModel));
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
