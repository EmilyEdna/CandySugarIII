using CandySugar.Com.Library;
using CandySugar.Com.Service;
using XExten.Advance.IocFramework;
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

    private async void ImportEvent(object sender, EventArgs e)
    {
        int Type = -1;
        var action = await DisplayActionSheet("导入类型", "确定", null, "漫画", "里番", "动漫", "车牌");
        if (action.Equals("漫画")) Type = 1;
        if (action.Equals("里番")) Type = 2;
        if (action.Equals("动漫")) Type = 4;
        if (action.Equals("车牌")) Type = 6;
        if (Type > 0)
        {
            PickAndShow(Type);
        }

    }
    public async void PickAndShow(int type)
    {
        var result = await FilePicker.Default.PickAsync(PickOptions.Default);
        if (result != null)
        {
            if (result.FileName.EndsWith("dat", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var model = (await reader.ReadToEndAsync()).ToModel<List<CollectModel>>();
                var service = IocDependency.Resolve<ICandyService>();
                foreach (var item in model)
                {
                    item.Category = type;
                    await service.Add(item);
                }
            }
        }
       
    }
}