using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VectorEditor.shapes
{
    public class Rectangle: Shape
    {
        public override void Draw (Graphics g)
        {
            DrawWithTransformations(g, (g) =>
            {
                using (var fillBrush = new SolidBrush(Color.FromArgb((int)(Opacity * 255), FillColor)))
                using (var strokePen = new Pen(StrokeColor, StrokeWidth))
                {
                    g.FillRectangle(fillBrush, Position.X, Position.Y, Size.width, Size.height);
                    g.DrawRectangle(strokePen, Position.X, Position.Y, Size.width, Size.height);
                    if (IsSelected)
                    {
                        DrawSelection(g);
                    }
                }
            }
            );
        }

        private void DrawSelection(Graphics g)
        {
            using (var selectedPen = new Pen(Color.Red, StrokeWidth)
            {
                DashPattern = new float[] { 3, 3 },
                DashStyle = DashStyle.Custom
            })
            {
                g.DrawRectangle(selectedPen, Position.X, Position.Y, Size.width, Size.height);
            }
            DrawTranformHandles(g);
        }

        private void DrawTranformHandles(Graphics g)
        {
            var bounds = GetBounds();
            var center = GetRotationCenter();
            
            var rotationHandle = new PointF(center.X, bounds.Y - 20);
            using (var handleBrush = new SolidBrush(Color.Blue))
            using (var handlePen = new Pen(Color.Blue,1))
            {
                g.DrawLine(handlePen, center, rotationHandle);
                g.FillEllipse(handleBrush, rotationHandle.X - 4, rotationHandle.Y - 4, 8, 8);
                g.FillEllipse(handleBrush, rotationHandle.X - 4, rotationHandle.Y - 4, 8, 8);
                DrawScaleHandles(g, bounds);
            }
        }
        private void DrawScaleHandles(Graphics g, RectangleF bounds)
        {
            var corners = new[]
            {
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Right, bounds.Top),
                new PointF(bounds.Right, bounds.Bottom),
                new PointF(bounds.Left, bounds.Bottom)
            };
            using (var handleBrush = new SolidBrush(Color.Green))
            {
                foreach (var corner in corners)
                {
                    g.FillRectangle(handleBrush, corner.X - 3, corner.Y - 3, 6, 6);
                }
            }
        }

        public override bool ContainsPoint(PointF point)
        {
            var localPoint = TransformPoint(point, true);
            return localPoint.X >= Position.X && localPoint.X <= Position.X + Size.width &&
                localPoint.Y >= Position.Y && localPoint.Y <= Position.Y + Size.height;
        }

        public override RectangleF GetBounds()
        {
            if (Rotation == 0 && Scale.X == 0 && Scale.Y == 0)
            {
                return new RectangleF(Position.X,Position.Y, Size.width, Size.height);
            }

            var corners = new PointF[]
            {
                new PointF(Position.X, Position.Y),
                new PointF(Position.X+Size.width,Position.Y),
                new PointF(Position.X+Size.width, Position.Y+Size.height),
                new PointF(Position.X,Position.Y+Size.height)
            };

            using (var matrix = GetTransfromationMatrix())
            {
                matrix.TransformPoints(corners);
            }
            float minX = corners.Min(p => p.X);
            float minY = corners.Min(p => p.Y);
            float maxX = corners.Max(p => p.X);
            float maxY = corners.Max(p => p.Y);

            return new RectangleF(minX,minY, maxX-minX, maxY-minY);
        }

        public bool IsScaleHandleClicked(PointF point)
        {
            if (!IsSelected) return false;

            var bounds = GetBounds();
            var corners = new[]
            {
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Right, bounds.Top),
                new PointF(bounds.Right, bounds.Bottom),
                new PointF(bounds.Left, bounds.Bottom)
            };
            foreach (var cor in corners)
            {
                var transformedCorner = TransformPoint(cor);
                var handleRect = new RectangleF(transformedCorner.X - 6, transformedCorner.Y - 6, 12, 12);
                if (handleRect.Contains(point)) { return true; }
            }
            return false;
        }
    }
}
