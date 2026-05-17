using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Extensions;

namespace CandySugar.Com.Controls
{
    public class LeftPage : Border, IPageAttachment
    {
        public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

        public static readonly BindableProperty IsPresentedProperty =
            BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(LeftPage), defaultValue: false,
                propertyChanged: (bo, ov, nv) => (bo as LeftPage).AlignLeftSheet());

        public bool DisablePageWhenOpened { get => (bool)GetValue(DisablePageWhenOpeneProperty); set => SetValue(DisablePageWhenOpeneProperty, value); }

        public static readonly BindableProperty DisablePageWhenOpeneProperty =
            BindableProperty.Create(
                nameof(DisablePageWhenOpened),
                typeof(bool), typeof(LeftPage), defaultValue: true);

        public bool CloseOnTapOutside { get => (bool)GetValue(CloseOnTapOutsideProperty); set => SetValue(CloseOnTapOutsideProperty, value); }

        public static readonly BindableProperty CloseOnTapOutsideProperty =
            BindableProperty.Create(
                nameof(CloseOnTapOutside),
                typeof(bool), typeof(LeftPage), defaultValue: true);

        public CandyUIPage AttachedPage { get; set; }
        public AttachmentLocation AttachmentPosition => AttachmentLocation.Front;
        public View Body { get; set; }
        private TapGestureRecognizer CloseGestureRecognizer = new();
        public void OnAttached(CandyUIPage page)
        {
            AttachedPage = page;
            page.SizeChanged += (s, e) => { AlignLeftSheet(false); };
            Init();
        }
        protected virtual void Init()
        {
            this.Padding = 0;
            this.WidthRequest = 0;
            this.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.HorizontalOptions = LayoutOptions.StartAndExpand;
            this.StrokeThickness = 0;
            this.HeightRequest = DeviceDisplay.Current.MainDisplayInfo.Height;
            var Stack = new VerticalStackLayout()
            {
                Children =
                {
                    Body
                }
            };
            Stack.Background.SetDynamicResource(VerticalStackLayout.BackgroundProperty, "BasicPage");
            this.Content = Stack;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => IsPresented = !IsPresented;
            Body.GestureRecognizers.Add(tapGestureRecognizer);
            AlignLeftSheet(false);

            CloseGestureRecognizer.Tapped += (s, e) => IsPresented = false;
        }
        protected virtual void OnOpened()
        {
            if (CloseOnTapOutside)
            {
                this.WidthRequest = 150;
                AttachedPage?.ContentBorder?.GestureRecognizers.Add(CloseGestureRecognizer);
            }
        }
        protected void UpdateDisabledStateOfPage()
        {
            if (AttachedPage?.Body != null && DisablePageWhenOpened)
            {
                AttachedPage.Body.InputTransparent = IsPresented;

                AttachedPage.Body.FadeTo(IsPresented ? .5 : 1);
            }
        }
        protected virtual void OnClosed()
        {
            if (CloseOnTapOutside)
            {
                this.WidthRequest = 0;
                AttachedPage?.ContentBorder?.GestureRecognizers.Remove(CloseGestureRecognizer);
            }
        }
        private void AlignLeftSheet(bool animate = true)
        {
            if (IsPresented) OnOpened();
            else OnClosed();
            if (animate) this.TranslateTo(0, -Y, 50);
            else this.TranslationX = this.X;
            UpdateDisabledStateOfPage();
        }
    }
}
