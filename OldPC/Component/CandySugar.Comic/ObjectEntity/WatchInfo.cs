namespace CandySugar.Comic.ObjectEntity
{
    public class WatchInfo:PropertyChangedBase
    {
        private int _Index;
        public int Index
        {
            get => _Index;
            set => SetAndNotify(ref _Index, value);
        }
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
    }
}
