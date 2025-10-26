using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VectorEditor.shapes
{
    public class Rectangle : Shape
    {
        private float _width = 100;
        private float _height = 80;

        public float Width
        {
            get => _width;
            set
            {
                _width = Math.Max(1, value);
                OnPropertyChanged(nameof(Width));
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                _height = Math.Max(1, value);
                OnPropertyChanged(nameof(Height));
            }
        }

        public override void Draw(Graphics graphics)
        {
            try
            {
                // Основная заливка
                using (var fillBrush = new SolidBrush(Color.FromArgb((int)(Opacity * 255), FillColor)))
                {
                    graphics.FillRectangle(fillBrush, X, Y, Width, Height);
                }

                // Обводка
                using (var strokePen = new Pen(StrokeColor, StrokeWidth))
                {
                    graphics.DrawRectangle(strokePen, X, Y, Width, Height);
                }

                // Выделение если выбрано
                if (IsSelected)
                {
                    DrawSelection(graphics);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Rectangle draw error: {ex.Message}");
            }
        }

        private void DrawSelection(Graphics graphics)
        {
            try
            {
                using (var selectedPen = new Pen(Color.Red, 1)
                {
                    DashPattern = new float[] { 3, 3 }
                })
                {
                    graphics.DrawRectangle(selectedPen, X, Y, Width, Height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Selection drawing error: {ex.Message}");
            }
        }

        public override bool ContainsPoint(PointF point)
        {
            return point.X >= X && point.X <= X + Width &&
                   point.Y >= Y && point.Y <= Y + Height;
        }

        public override RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width, Height);
        }
    }
}