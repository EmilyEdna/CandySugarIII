namespace CandySugar.WallPaper
{
    public class Module
    {
        public static Module IocModule { get; set; }
        private static IContainer Container { get; set; }
        public Module()
        {
            IocModule = this;
            Container = new Container();
            Container.Register(typeof(WallhavView), Reuse.Singleton);
            Container.Register(typeof(WallhavViewModel), Reuse.Singleton);
            Container.Register(typeof(WallchanView), Reuse.Singleton);
            Container.Register(typeof(WallchanViewModel), Reuse.Singleton);
        }

        public T Resolve<T>() where T : UserControl
        {
            var Ctrl = (UserControl)Container.Resolve(typeof(T));
            var VM = this.GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{typeof(T).Name}Model");
            Ctrl.DataContext = Container.Resolve(VM);
            return (T)Ctrl;
        }
    }

}
