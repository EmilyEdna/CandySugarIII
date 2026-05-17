namespace CandySugar.Cosplay.ObjectEntity
{
    public class MenuInfo : PropertyChangedBase
    {
        private int _Key;
        public int Key
        {
            get => _Key;
            set => SetAndNotify(ref _Key, value);
        }
        private string _Value;
        public string Value
        {
            get => _Value;
            set => SetAndNotify(ref _Value, value);
        }
    }
}
