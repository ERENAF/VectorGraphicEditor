using System;
using System.Drawing;
using System.ComponentModel;

namespace VectorEditor.shapes
{
    public abstract class Shape : INotifyPropertyChanged
    {
        private float _x;
        private float _y;
        private Color _fillColor = Color.LightBlue;
        private Color _strokeColor = Color.Black;
        private float _strokeWidth = 2.0f;
        private float _opacity = 1.0f;
        private bool _isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public float X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        public float Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        public PointF Position
        {
            get => new PointF(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Color FillColor
        {
            get => _fillColor;
            set { _fillColor = value; OnPropertyChanged(nameof(FillColor)); }
        }

        public Color StrokeColor
        {
            get => _strokeColor;
            set { _strokeColor = value; OnPropertyChanged(nameof(StrokeColor)); }
        }

        public float StrokeWidth
        {
            get => _strokeWidth;
            set { _strokeWidth = Math.Max(0.1f, value); OnPropertyChanged(nameof(StrokeWidth)); }
        }

        public float Opacity
        {
            get => _opacity;
            set { _opacity = Math.Max(0, Math.Min(1, value)); OnPropertyChanged(nameof(Opacity)); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public abstract void Draw(Graphics graphics);
        public abstract bool ContainsPoint(PointF point);
        public abstract RectangleF GetBounds();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}