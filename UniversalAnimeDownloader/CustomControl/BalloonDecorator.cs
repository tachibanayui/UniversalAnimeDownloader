using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniversalAnimeDownloader.CustomControl
{
    public class BalloonDecorator : Decorator
    {
        private static double _thickness = 0;
        private static int OpeningGap = 10;

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(BalloonDecorator));

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(BalloonDecorator));

        public static readonly DependencyProperty PointerLengthProperty =
            DependencyProperty.Register("PointerLength", typeof(double), typeof(BalloonDecorator),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CornerPositionProperty =
            DependencyProperty.Register("CornerPosition", typeof(string), typeof(BalloonDecorator),
            new FrameworkPropertyMetadata("Left", FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(BalloonDecorator),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public string CornerPosition
        {
            get { return (string)GetValue(CornerPositionProperty); }
            set { SetValue(CornerPositionProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public double PointerLength
        {
            get { return (double)GetValue(PointerLengthProperty); }
            set { SetValue(PointerLengthProperty, value); }
        }

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement child = Child;
            if (child != null)
            {
                double pLength = PointerLength;
                Rect innerRect = (CornerPosition == "Right") ?
                                        Rect.Inflate(new Rect(0, 0, Math.Max(0, arrangeSize.Width - pLength), arrangeSize.Height),
                                        -1 * _thickness, -1 * _thickness) :
                    Rect.Inflate(new Rect(pLength, 0, Math.Max(0, arrangeSize.Width - pLength), arrangeSize.Height),
                                 -1 * _thickness, -1 * _thickness);
                child.Arrange(innerRect);
            }

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement child = Child;
            Size size = new Size();
            if (child != null)
            {
                Size innerSize = new Size(Math.Max(0, constraint.Width - PointerLength), constraint.Height);
                child.Measure(innerSize);
                size.Width += child.DesiredSize.Width;
                size.Height += child.DesiredSize.Height;
            }

            Size borderSize = new Size(2 * _thickness, 2 * _thickness);
            size.Width += borderSize.Width + PointerLength;
            size.Height += borderSize.Height;

            return size;
        }

        protected override void OnRender(DrawingContext dc)
        {
            Rect rect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);

            dc.PushClip(new RectangleGeometry(rect));
            dc.DrawGeometry(Background, new Pen(BorderBrush, _thickness), CreateBalloonGeometry(rect));

            dc.Pop();
        }

        private StreamGeometry CreateBalloonGeometry(Rect rect)
        {
            double radius = Math.Min(CornerRadius, rect.Height / 2);
            double pointerLength = PointerLength;

            Point[] points = new Point[8];

            if (CornerPosition == "Left")
            {
                // All the points on the path
                points[0] = new Point(pointerLength + radius, 0);
                points[1] = new Point(rect.Width - radius, 0); // Top
                points[2] = new Point(rect.Width, radius);
                points[3] = new Point(rect.Width, rect.Height - radius); // Right
                points[4] = new Point(rect.Width - radius, rect.Height); // Bottom
                points[5] = new Point(pointerLength + radius, rect.Height); // Bottom
                points[6] = new Point(pointerLength, rect.Height - radius); // Left
                points[7] = new Point(pointerLength, radius); // Left

            }
            else
            {
                int pointerLength1 = 0;
                // All the points on the path
                {
                    points[0] = new Point(pointerLength1 + radius, 0);
                    points[1] = new Point(rect.Width - radius - PointerLength, 0); // Top

                    points[2] = new Point(rect.Width - PointerLength, radius);
                    points[3] = new Point(rect.Width - PointerLength, rect.Height - radius); // Right
                    points[4] = new Point(rect.Width - radius - PointerLength, rect.Height); // Bottom
                    points[5] = new Point(pointerLength1 + radius, rect.Height); // Bottom
                    points[6] = new Point(pointerLength1, rect.Height - radius); // Left
                    points[7] = new Point(pointerLength1, radius); // Left
                };
            }

            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.Nonzero;
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(points[0], true, true);
                ctx.LineTo(points[1], true, false);



                ctx.ArcTo(points[2], new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);


                ctx.LineTo(points[3], true, false);

                // Pointer
                if ((pointerLength > 0) && (CornerPosition == "Right"))
                {
                    ctx.LineTo(new Point(rect.Width, rect.Height), true, false);
                    ctx.LineTo(new Point(rect.Width - pointerLength, rect.Height), true, false);
                }


                ctx.ArcTo(points[4], new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
                ctx.LineTo(points[5], true, false);

                ctx.ArcTo(points[6], new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);

                // Pointer
                if ((pointerLength > 0) && (CornerPosition == "Left"))
                {
                    ctx.LineTo(rect.BottomLeft, true, false);
                    ctx.LineTo(new Point(pointerLength, rect.Height - radius - OpeningGap), true, false);
                }
                ctx.LineTo(points[7], true, false);

                ctx.ArcTo(points[0], new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);


            }
            return geometry;
        }
    }
}
