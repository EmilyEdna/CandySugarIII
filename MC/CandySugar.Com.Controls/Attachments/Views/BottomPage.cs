using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Extensions;
using UraniumUI.Material.Attachments;

namespace CandySugar.Com.Controls
{
    [ContentProperty(nameof(Body))]
    public class BottomPage : Border, IPageAttachment
    {
        public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

        public static readonly BindableProperty IsPresentedProperty =
            BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(BottomPage), defaultValue: false,
                propertyChanged: (bo, ov, nv) => (bo as BottomPage).AlignBottomSheet());

        public bool DisablePageWhenOpened { get => (bool)GetValue(DisablePageWhenOpeneProperty); set => SetValue(DisablePageWhenOpeneProperty, value); }

        public static readonly BindableProperty DisablePageWhenOpeneProperty =
            BindableProperty.Create(
                nameof(DisablePageWhenOpened),
                typeof(bool), typeof(BottomPage), defaultValue: true);

        public bool CloseOnTapOutside { get => (bool)GetValue(CloseOnTapOutsideProperty); set => SetValue(CloseOnTapOutsideProperty, value); }

        public static readonly BindableProperty CloseOnTapOutsideProperty =
            BindableProperty.Create(
                nameof(CloseOnTapOutside),
                typeof(bool), typeof(BottomPage), defaultValue: true);

        public CandyUIPage AttachedPage { get; set; }
        public AttachmentLocation AttachmentPosition => AttachmentLocation.Front;
        public View Body { get; set; }

        public View Header { get; set; }

        private TapGestureRecognizer CloseGestureRecognizer = new();

        public void OnAttached(CandyUIPage page)
        {
            Init();

            AttachedPage = page;
            page.SizeChanged += (s, e) => { AlignBottomSheet(false); };
        }
        protected virtual void Init()
        {
            Header ??= GenerateAnchor();
            Padding = 0;
            this.VerticalOptions = LayoutOptions.End;
            this.HorizontalOptions = LayoutOptions.Fill;
            this.StrokeThickness = 0;
            this.StrokeShape = new RoundRectangle { CornerRadius = 1 };
            this.Content = new VerticalStackLayout()
            {
                Children =
                {
                    Header,
                    Body
                }
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => IsPresented = !IsPresented;
            Header.GestureRecognizers.Add(tapGestureRecognizer);
            Header.BackgroundColor = this.BackgroundColor;
            AlignBottomSheet(false);

            CloseGestureRecognizer.Tapped += (s, e) => IsPresented = false;
        }
        protected virtual View GenerateAnchor()
        {
            var anchor = new ContentView
            {
                HorizontalOptions = LayoutOptions.Fill,
                Padding = 10,
                Content = new BoxView
                {
                    HeightRequest = 2,
                    CornerRadius = 2,
                    WidthRequest = 50,
                    Color = this.BackgroundColor?.ToSurfaceColor() ?? Colors.Gray,
                    HorizontalOptions = LayoutOptions.Center,
                }
            };

            return anchor;
        }
        protected virtual void OnOpened()
        {
            if (CloseOnTapOutside)
            {
                AttachedPage?.ContentBorder?.GestureRecognizers.Add(CloseGestureRecognizer);
            }
        }
        protected virtual void OnClosed()
        {
            if (CloseOnTapOutside)
            {
                AttachedPage?.ContentBorder?.GestureRecognizers.Remove(CloseGestureRecognizer);
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
        private void AlignBottomSheet(bool animate = true)
        {
            double y = this.Height - Header.Height;
            if (IsPresented)
            {
                y = 0;
                OnOpened();
            }
            else
            {
                OnClosed();
            }

            if (animate)
            {
                this.TranslateTo(this.X, y, 50);

            }
            else
            {
                this.TranslationY = y;
            }

            UpdateDisabledStateOfPage();
        }
    }
}
