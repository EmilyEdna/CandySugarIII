using System.Collections;

namespace CandySugar.Com.Library
{
    public class ToolBarAttach
    {

        public static void SetBarSource(BindableObject target)
        {
            target.SetValue(BarSourceProperty, null);
        }
        public static IEnumerable GetBarSource(BindableObject target)
        {
            return (IEnumerable)target.GetValue(BarSourceProperty);
        }

        public static readonly BindableProperty BarSourceProperty =
            BindableProperty.CreateAttached("BarSource", typeof(IEnumerable), typeof(ToolBarAttach), null, propertyChanged: OnPropertyChanged);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not ContentPage)
                return;
            var page = (ContentPage)bindable;
            var result = GetBarSource(bindable);
            foreach (var item in result)
            {
                var NameProperty = item.GetType().GetProperties().FirstOrDefault(t => t.Name.Equals("Name"));
                var RouteProperty = item.GetType().GetProperties().FirstOrDefault(t => t.Name.Equals("Route"));
                if (NameProperty != null&&RouteProperty!=null)
                {
                    page.ToolbarItems.Add(new ToolbarItem
                    {
                        Order = ToolbarItemOrder.Secondary,
                        Text = NameProperty.GetValue(item).ToString(),
                        CommandParameter = RouteProperty.GetValue(item).ToString(),
                        Command = ((dynamic)page.BindingContext).CatalogCommand
                    });
                }
            }
        }
    }
}
