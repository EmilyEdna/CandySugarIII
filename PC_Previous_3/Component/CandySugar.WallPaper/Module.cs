namespace CandySugar.WallPaper
{
    public class Module
    {
        public static Module IocModule { get; set; }

        public Module()
        {
            IocModule = this;

            IocDependency.Register(typeof(Index1View));
            IocDependency.Register(typeof(Index1ViewModel));
            IocDependency.Register(typeof(Index2View));
            IocDependency.Register(typeof(Index2ViewModel));
            IocDependency.Register(typeof(Index3View));
            IocDependency.Register(typeof(Index3ViewModel));
        }

        public T Resolve<T>() where T : UserControl
        {
            var Ctrl = (UserControl)IocDependency.Resolve(typeof(T));
            var VMName = this.GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{typeof(T).Name}Model");
            var VM = (BasicObservableObject)IocDependency.Resolve(VMName);
            VM.InitSearch();
            Ctrl.DataContext = VM;
            return (T)Ctrl;
        }
    }

}
