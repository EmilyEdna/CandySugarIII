using CandySugar.Com.Library;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages;

public partial class IndexView : Shell
{
    public IndexView()
    {
        InitializeComponent();
        this.GetType().Assembly.ExportedTypes.Where(t => t.BaseType == typeof(ContentPage)).ForEnumerEach(item =>
        {
            Routing.RegisterRoute(Extend.RouteMap[item.Name], item);
        });

        this.Appearing += InitEvent;
    }

    private async void InitEvent(object sender, EventArgs e)
    {
        await Permissions.RequestAsync<Permissions.StorageWrite>();
        await Permissions.RequestAsync<Permissions.StorageRead>();
    }
}