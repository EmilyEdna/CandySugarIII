namespace CandySugar.Com.Controls
{
    public class DropView : ContentView, IPageAttachment
    {
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(DropView),
        propertyChanged: (bo, ov, nv) => (bo as DropView).OnPropertyChanged(nameof(Title)));

        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        public static readonly BindableProperty IconImageSourceProperty =
            BindableProperty.Create(nameof(IconImageSource), typeof(ImageSource), typeof(DropView),
                propertyChanged: (bo, ov, nv) => (bo as DropView).OnPropertyChanged(nameof(IconImageSource)));

        public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

        public static readonly BindableProperty IsPresentedProperty =
            BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(DropView), defaultValue: false,
                propertyChanged: (bo, ov, nv) => (bo as DropView).SlideToState((bool)nv));

        public CandyUIPage AttachedPage { get; protected set; }
        public AttachmentLocation AttachmentPosition => AttachmentLocation.Behind;
        protected ToolbarItem toolbarItem = new ToolbarItem();

        public DropView()
        {
            this.VerticalOptions = LayoutOptions.Fill;

            this.Padding = new Thickness(20, 0, 20, 30);
        }

        public void OnAttached(CandyUIPage attachedPage)
        {
            AttachedPage = attachedPage;
            if (Shell.Current?.BackgroundColor != null)
            {
                this.BackgroundColor = Shell.Current.BackgroundColor;
            }
            if (Shell.Current?.Background != null)
            {
                this.Background = Shell.Current.Background;
            }

            this.Content.VerticalOptions = LayoutOptions.Start;

            toolbarItem.SetBinding(ToolbarItem.IconImageSourceProperty, new Binding(nameof(IconImageSource), source: this));
            toolbarItem.SetBinding(ToolbarItem.TextProperty, new Binding(nameof(Title), source: this));
            toolbarItem.Clicked += (s, e) => IsPresented = !IsPresented;

            AttachedPage.ToolbarItems.Add(toolbarItem);
        }

        protected virtual void SlideToState(bool isPresented)
        {
            foreach (DropView backdrop in AttachedPage.Attachments.Where(x => x is DropView))
            {
                backdrop.IsVisible = isPresented && backdrop == this;
            }

            AttachedPage.ContentBorder.TranslateTo(0, isPresented ? this.Content.Height : 0);
        }
    }
}
