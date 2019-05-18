using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.VisualTree;
using SkiaSharp;

namespace ClipToBoundsIssueRepro.Controls
{
    internal struct CustomDrawOperation : ICustomDrawOperation
    {
        private readonly Action<SKCanvas, double, double> _draw;
        private readonly double _width;
        private readonly double _height;

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public CustomDrawOperation(Action<SKCanvas, double, double> draw, double width, double height)
        {
            _draw = draw;
            _width = width;
            _height = height;
            Bounds = new Rect(0, 0, width, height);
        }

        public void Render(IDrawingContextImpl context)
        {
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas != null && _draw != null)
            {
                _draw(canvas, _width, _height);
            }
        }

        public void Dispose()
        {
        }
    }

    public class ZoomControl : Border
    {
        public static readonly StyledProperty<bool> CustomDrawProperty =
            AvaloniaProperty.Register<ZoomControl, bool>(nameof(CustomDraw));

        public bool CustomDraw
        {
            get { return GetValue(CustomDrawProperty); }
            set { SetValue(CustomDrawProperty, value); }
        }

        public ZoomControl()
        {
            PointerPressed += (sender, e) => this.InvalidateVisual();
            PointerMoved += (sender, e) => this.InvalidateVisual();
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            this.Focus();
        }

        private void DrawSkia(SKCanvas canvas, double width, double height)
        {
            var bounds = new SKRect((float)0, (float)0, (float)width, (float)height);
            
            var brush = new SKPaint()
            {
                IsAntialias = true,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = new SKColor(128, 128, 128, 255),
                TextAlign = SKTextAlign.Left
            };

            canvas.DrawRect(bounds, brush);
            
            var pen = new SKPaint()
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = (float)2.0,
                Color = new SKColor(255, 0, 0, 255),
                StrokeCap = SKStrokeCap.Butt,
                PathEffect = null
            };

            canvas.DrawLine(new SKPoint((float)-100, (float)(height / 2)), new SKPoint((float)width + 100, (float)(height / 2)), pen);

            brush.Dispose();
            pen.Dispose();
        }

        private void DrawAvalonia(DrawingContext context, double width, double height)
        {
            var bounds = new Rect(0.0, 0.0, width, height);
            var boundsState = context.PushClip(bounds);

            var brush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));

            context.FillRectangle(brush, bounds);

            var pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), 2);

            context.DrawLine(pen, new Point(-100, height / 2), new Point(width + 100, (height / 2)));

            boundsState.Dispose();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (CustomDraw)
            {
                context.Custom(new CustomDrawOperation(DrawSkia, Bounds.Width, Bounds.Height));
            }
            else
            {
                DrawAvalonia(context, Bounds.Width, Bounds.Height);
            }
        }
    }
}
