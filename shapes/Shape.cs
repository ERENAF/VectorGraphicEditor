using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VectorEditor.shapes
{
    public struct Scale
    {
        public float X;
        public float Y;
    };
    public struct Size
    {
        public float width;
        public float height;
    }
    public abstract class Shape
    {
        public Guid Id;
        private PointF _position;
        private float _rotation;
        private Size _size;
        private Scale _scale;
        private Color _fillColor = Color.White;
        private Color _strokeColor = Color.Black;
        private float _strokeWidth = 1.0f;
        private float _opacity = 1.0f;
        private bool _isSelected;
        
        public PointF Position
        {
            get => _position; set => _position = value;
        }
        public float Rotation
        {
            get => _rotation; set => _rotation = value;
        }
        public Size Size
        {
            get => _size;
            set
            {
                if (value.width < 0 || value.height < 0)
                {
                    throw new ArgumentException("Value can't be negative");
                }
                _size = value;
            }
        }
        public Scale Scale
        {
            get => _scale; 
            set
            {

                if (value.X < 0 || value.Y < 0)
                {
                    throw new ArgumentException("Scale can't be negative");
                }
                _scale = value;
            }
        }
        public Color FillColor
        {
            get => _fillColor; set => _fillColor = value;
        }
        public Color StrokeColor
        {
            get => _strokeColor; set => _strokeColor = value;
        }
        public float StrokeWidth
        {
            get => _strokeWidth;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Stroke width can't be negative");
                }
                _strokeWidth = value;
            }
        }
        public float Opacity
        {
            get => _opacity;
            set
            {
                if (value < 0  || value > 100)
                {
                    throw new ArgumentException("value should be from 0 to 100");
                }
                _opacity = value;
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }
        public Color ApplyOpacity()
        {
            return Color.FromArgb((int)(Opacity * 255),FillColor);
        }
        public virtual PointF GetRotationCenter()
        {
            var bounds = GetBounds();
            return new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
        }
        protected virtual void ApplyTransformations(Graphics graphics)
        {
            if (Rotation == 0 && Scale.X == 1 && Scale.Y == 1)
            {
                return;
            }
            var center = GetRotationCenter();

            graphics.TranslateTransform(center.X, center.Y);
            graphics.RotateTransform(Rotation);
            graphics.ScaleTransform(Scale.X, Scale.Y);
            graphics.TranslateTransform(-center.X, -center.Y);
        }
        protected virtual Matrix GetTransfromationMatrix()
        {
            var matrix = new Matrix();
            var center = GetRotationCenter();

            matrix.Translate(center.X, center.Y);
            matrix.Rotate(Rotation);
            matrix.Scale(Scale.X, Scale.Y);
            matrix.Translate(-center.X, -center.Y);
            return matrix;
        }
        protected virtual PointF TransformPoint(PointF point, bool inverse = false)
        {
            using (var matrix = GetTransfromationMatrix())
            {
                if (inverse)
                {
                    var inverseMatrix = matrix.Clone();
                    inverseMatrix.Invert();
                    var points = new[] { point };
                    inverseMatrix.TransformPoints(points);
                    return points[0];
                }
                else
                {
                    var points = new[] { point };
                    matrix.TransformPoints(points);
                    return points[0];
                }
            }
        }

        protected void DrawWithTransformations(Graphics g, Action<Graphics> drawAction)
        {
            var saveState = g.Save();
            try
            {
                ApplyTransformations(g);
                drawAction(g);
            }
            finally
            {
                g.Restore(saveState);
            }
        }
        public abstract void Draw(Graphics g);
        public abstract bool ContainsPoint(PointF point);
        public abstract RectangleF GetBounds();
    }

}
