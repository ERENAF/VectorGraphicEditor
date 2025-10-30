using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEditor.shapes
{
    public class Ellipse : Shape
    {
        public override void Draw(Graphics graphics)
        {
            RotationTransform(graphics);
            using (var fillBrush = new SolidBrush(Color.FromArgb((int)(Opacity * 255), FillColor)))
            using (var strokePen = new Pen(StrokeColor, StrokeWidth))
            {
                if (IsSelected)
                {
                    using (var selectedPen = new Pen(Color.Red, StrokeWidth) { DashPattern = new float[] { 3, 3 } })
                    {
                        graphics.DrawEllipse(selectedPen, X, Y, Width*Scale, Height*Scale);
                    }
                }

                graphics.FillEllipse(fillBrush, X, Y, Width*Scale, Height * Scale);
                graphics.DrawEllipse(strokePen, X, Y, Width * Scale, Height * Scale);
            }
        }

        public override bool ContainsPoint(PointF point)
        {
            float centerX = X + Width * Scale/ 2;
            float centerY = Y + Height * Scale/ 2;
            float dx = point.X - centerX;
            float dy = point.Y - centerY;

            return (dx * dx) / (Width * Width * Scale * Scale/ 4) + (dy * dy) / (Height * Height * Scale * Scale/ 4) <= 1;
        }

        public override RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width* Scale, Height* Scale);
        }
    }
}
