using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages;

public partial class IndexView : Shell
{
    public IndexView()
    {
        InitializeComponent();
        this.BindingContext = new IndexViewModel();
        this.GetType().Assembly.ExportedTypes.Where(t => t.BaseType == typeof(ContentPage)).ForEnumerEach(item =>
        {
            Routing.RegisterRoute(item.Name, item);
        });
    }

   
    private async void RouteEvent(object sender, EventArgs e)
    {
        await Current.GoToAsync((sender as Button).CommandParameter.ToString());
    }
}