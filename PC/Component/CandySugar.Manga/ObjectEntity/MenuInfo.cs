namespace CandySugar.Manga.ObjectEntity
{
    public class MenuInfo : PropertyChangedBase
    {
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetAndNotify(ref _Name, value);
        }
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
    }
}
