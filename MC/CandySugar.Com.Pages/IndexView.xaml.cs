using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages;

public partial class IndexView : Shell
{
    public IndexView()
    {
        InitializeComponent();
        this.GetType().Assembly.ExportedTypes.Where(t => t.BaseType == typeof(ContentPage)).ForEnumerEach(item =>
        {
            Routing.RegisterRoute(item.Name, item);
        });
    }
}