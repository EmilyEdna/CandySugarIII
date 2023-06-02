using CommunityToolkit.Maui.Layouts;
using System.Collections;
using System.Collections.Specialized;

namespace CandySugar.Com.Controls
{
    public class UniformGrid : UniformItemsLayout
    {
        public UniformGrid()
        {
            Render();
        }

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty =
              BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(UniformGrid),
                  propertyChanged: (sender, oldValue, newValue) => ((UniformGrid)sender).OnItemsSourceChanged((IList)oldValue, (IList)newValue));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(UniformGrid), propertyChanged: (sender, oldValue, newValue) => ((UniformGrid)sender).Render());

        public bool AutoColumns
        {
            get { return (bool)GetValue(AutoColumnsProperty); }
            set { SetValue(AutoColumnsProperty, value); }
        }

        public static readonly BindableProperty AutoColumnsProperty =
            BindableProperty.Create(nameof(AutoColumns), typeof(bool), typeof(UniformGrid), false);

        public bool AutoRows
        {
            get { return (bool)GetValue(AutoRowsProperty); }
            set { SetValue(AutoRowsProperty, value); }
        }

        public static readonly BindableProperty AutoRowsProperty =
            BindableProperty.Create(nameof(AutoRows), typeof(bool), typeof(UniformGrid), false);


        private void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemChanged;
            }
            if (newValue is INotifyCollectionChanged n)
            {
                n.CollectionChanged += ItemChanged;
            }
            Render();
        }

        private void ItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            AddTemplate(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            RemoveTemplate(item);
                        }
                    }
                    break;
                default:
                    Render();
                    break;
            }
        }

        protected void Render()
        {
            if (ItemsSource?.Count > 0)
            {
                this.Children.Clear();
                this.MaxColumns = this.AutoColumns ? ItemsSource.Count : this.MaxColumns;
                this.MaxRows = this.AutoRows ? ItemsSource.Count : this.MaxRows;
                foreach (var item in ItemsSource)
                {
                    AddTemplate(item);
                }
            }
        }

        protected void AddTemplate(object item)
        {
            var view = ItemTemplate?.CreateContent() as View;
            if (view != null)
            {
                view.BindingContext = item; 
                this.Children.Add(view);
            }
        }

        protected void RemoveTemplate(object item)
        {
            var existing = this.Children.FirstOrDefault(t => t is View view && view.BindingContext == item);
            if (existing != null) this.Children.Remove(existing);
        }

    }
}
