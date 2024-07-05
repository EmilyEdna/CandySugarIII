namespace CandySugar.Comic.Model
{
    public partial class WatchInfo:BasicObservableObject
    {
        [ObservableProperty]
        private int _Index;
        [ObservableProperty]
        private string _Route;
        [ObservableProperty]
        private string _Preview;

    }
}
