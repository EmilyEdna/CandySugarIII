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
    public class LeftView : Border, IViewAttachment
    {
        public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

        public static readonly BindableProperty IsPresentedProperty =
            BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(LeftView), defaultValue: false,
                propertyChanged: (bo, ov, nv) => (bo as LeftView).AlignLeftSheet());

        public bool DisablePageWhenOpened { get => (bool)GetValue(DisablePageWhenOpeneProperty); set => SetValue(DisablePageWhenOpeneProperty, value); }

        public static readonly BindableProperty DisablePageWhenOpeneProperty =
            BindableProperty.Create(
                nameof(DisablePageWhenOpened),
                typeof(bool), typeof(LeftView), defaultValue: true);

        public bool CloseOnTapOutside { get => (bool)GetValue(CloseOnTapOutsideProperty); set => SetValue(CloseOnTapOutsideProperty, value); }

        public static readonly BindableProperty CloseOnTapOutsideProperty =
            BindableProperty.Create(
                nameof(CloseOnTapOutside),
                typeof(bool), typeof(LeftView), defaultValue: true);

        public CandyUIView AttachedView { get; set; }
        public AttachmentLocation AttachmentPosition => AttachmentLocation.Front;
        public View Body { get; set; }
        private TapGestureRecognizer CloseGestureRecognizer = new();
        public void OnAttached(CandyUIView view)
        {
            AttachedView = view;
            view.SizeChanged += (s, e) => { AlignLeftSheet(false); };
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
            this.Content = new VerticalStackLayout()
            {
                Children =
                {
                    Body
                }
            };

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
                this.WidthRequest = 250;
                AttachedView?.ContentBorder?.GestureRecognizers.Add(CloseGestureRecognizer);
            }
        }
        protected void UpdateDisabledStateOfPage()
        {
            if (AttachedView?.Body != null && DisablePageWhenOpened)
            {
                AttachedView.Body.InputTransparent = IsPresented;

                AttachedView.Body.FadeTo(IsPresented ? .5 : 1);
            }
        }
        protected virtual void OnClosed()
        {
            if (CloseOnTapOutside)
            {
                this.WidthRequest = 0;
                AttachedView?.ContentBorder?.GestureRecognizers.Remove(CloseGestureRecognizer);
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
