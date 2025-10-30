using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEditor.shapes
{
    public class Line : Shape
    {
        private float _endX = 100;
        private float _endY = 100;

        public float EndX
        {
            get => _endX;
            set { _endX = value; OnPropertyChanged(nameof(EndX)); }
        }

        public float EndY
        {
            get => _endY;
            set { _endY = value; OnPropertyChanged(nameof(EndY)); }
        }

        public override void Draw(Graphics graphics)
        {
            RotationTransform(graphics);
            using (var strokePen = new Pen(StrokeColor, StrokeWidth))
            {
                if (IsSelected)
                {
                    using (var selectedPen = new Pen(Color.Red, StrokeWidth) { DashPattern = new float[] { 3, 3 } })
                    {
                        graphics.DrawLine(selectedPen, X, Y, EndX, EndY);
                    }
                }

                graphics.DrawLine(strokePen, X, Y, EndX, EndY);
            }
        }

        public override bool ContainsPoint(PointF point)
        {
            float distance = DistanceToLine(point, new PointF(X, Y), new PointF(EndX, EndY));
            return distance <= StrokeWidth + 5;
        }
        private float DistanceToLine(PointF point, PointF lineStart, PointF lineEnd)
        {
            float A = point.X - lineStart.X;
            float B = point.Y - lineStart.Y;
            float C = lineEnd.X - lineStart.X;
            float D = lineEnd.Y - lineStart.Y;

            float dot = A * C + B * D;
            float lenSq = C * C + D * D;
            float param = (lenSq != 0) ? dot / lenSq : -1;

            float xx, yy;

            if (param < 0)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else
            {
                xx = lineStart.X + param * C;
                yy = lineStart.Y + param * D;
            }

            float dx = point.X - xx;
            float dy = point.Y - yy;
            return (float)Math.Sqrt(dx * dx + dy * dy)*Scale;
        }
        public override RectangleF GetBounds()
        {
            float minX = Math.Min(X, EndX);
            float minY = Math.Min(Y, EndY);
            float maxX = Math.Max(X, EndX);
            float maxY = Math.Max(Y, EndY);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
