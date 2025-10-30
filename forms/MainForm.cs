using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorEditor.commands;
using VectorEditor.managers;
using VectorEditor.shapes;

namespace VectorEditor.forms
{
    public partial class MainForm : Form
    {
        private readonly List<Shape> _shapes = new List<Shape>();
        private readonly ShapeManager _shapemanager = new ShapeManager();

        private Shape _selectedshape;
        private ShapeType _currentShapeType = ShapeType.Rectangle;
        private Color _currentColor = Color.Blue;

        private bool _IsDrawing;
        private bool _IsMoving;
        private bool _IsRotating;
        private PointF _startPos;
        private PointF _finishPos;


        private NumericUpDown posXNUD;
        private NumericUpDown posYNUD;
        private NumericUpDown angleNUD;
        private NumericUpDown scaleNUD;
        private Panel canvas;
        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            _shapemanager.CommandExecuted += (s, e) => canvas.Invalidate();
        }

        private void SetupUI()
        {
            Text = "vector editor";
            Size = new Size(1000, 700);
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 200
            };

            var toolPanel = new Panel { Dock = DockStyle.Left, Width = 200 };

            var shapeCmbbtn = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Top = 10,
                Left = 10,
                Width = 80
            };
            shapeCmbbtn.Items.AddRange(Enum.GetNames(typeof(ShapeType)));
            shapeCmbbtn.SelectedIndex = 0;
            shapeCmbbtn.Click += (s, e) =>
            {
                if (shapeCmbbtn.SelectedIndex >= 0)
                {
                    _currentShapeType = (ShapeType)shapeCmbbtn.SelectedIndex;
                }
            };

            var colorLabel = new Label { Text = "Color", Top = 110, Left = 10 };
            var colorBtn = new Button { Text = "Choose", Top = 130, Left = 10, Width = 80 };
            colorBtn.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog { Color = _currentColor };
                if (colorDialog.ShowDialog() == DialogResult.OK) { _currentColor = colorDialog.Color; }
            };

            var posLabel = new Label { Text = "Position", Top = 320, Left = 10 };

            posXNUD = new NumericUpDown { Top = 340, Left = 10, Minimum = 0, Maximum = 1000, Value = 0, Increment = 1, Width = 60 };
            posXNUD.ValueChanged += (s, e) =>
            {
                if (_selectedshape != null)
                {
                    var finpos = new PointF((float)posXNUD.Value, _selectedshape.Position.Y);
                    var command = new MoveShapeCommand(_selectedshape, _selectedshape.Position, finpos);
                    _shapemanager.ExecuteCommand(command);
                    canvas.Invalidate();
                }
            };

            posYNUD = new NumericUpDown { Top = 340, Left = 80, Minimum = 0, Maximum = 1000, Value = 0, Increment = 1, Width = 60};
            posYNUD.ValueChanged += (s, e) =>
            {
                if (_selectedshape != null)
                {
                    var finpos = new PointF(_selectedshape.X, (float)posYNUD.Value);
                    var command = new MoveShapeCommand(_selectedshape, _selectedshape.Position, finpos);
                    _shapemanager.ExecuteCommand(command);
                    canvas.Invalidate();
                }
            };



            var scaleLabel = new Label { Text = "Scale: ", Top = 360, Left = 10 };
            scaleNUD = new NumericUpDown { Text = "Scale: ", Top = 380, Left = 10 , Minimum = 0, Maximum = 1000, Value = 0, Increment = 1};
            scaleNUD.ValueChanged += (s, e) =>
            {
                if (_selectedshape != null)
                {
                    var command = new ScaleCommand(_selectedshape.Scale, (float)scaleNUD.Value, _selectedshape);
                    _shapemanager.ExecuteCommand(command);
                    canvas.Invalidate();
                }
            };

            var angleLabel = new Label { Text = "Rotation: ", Top = 420, Left = 10 };
            angleNUD = new NumericUpDown { Text = "Rotation: ", Top = 440, Left = 10 , Minimum = 0, Maximum = 360, Value = 0, Increment = 1};
            angleNUD.ValueChanged += (s, e) =>
            {
                if (_selectedshape != null)
                {
                    var command = new RotationCommand(_selectedshape, (float)angleNUD.Value);
                    _shapemanager.ExecuteCommand(command);
                    canvas.Invalidate();
                }
            };


            var delBtn = new Button { Text = "Delete", Top = 170, Left = 10, Width = 80 };
            var bringToFrontBtn = new Button { Text = "To Front", Top = 200, Left = 10, Width = 80 };
            var sendToBackBtn = new Button { Text = "To Back", Top = 230, Left = 10, Width = 80 };
            var undoBtn = new Button { Text = "Undo", Top = 260, Left = 10, Width = 80 };
            var redoBtn = new Button { Text = "Redo", Top = 290, Left = 10, Width = 80 };

            delBtn.Click += DelSelectedShape;
            bringToFrontBtn.Click += BringToFront;
            sendToBackBtn.Click += SendToBack;
            undoBtn.Click += (s, e) => _shapemanager.Undo();
            redoBtn.Click += (s, e) => _shapemanager.Redo();


            toolPanel.Controls.AddRange(new Control[] {
                shapeCmbbtn, colorLabel,colorBtn,delBtn,bringToFrontBtn,sendToBackBtn,undoBtn,redoBtn, angleLabel,angleNUD, scaleNUD, scaleLabel, posLabel, posXNUD, posYNUD
            });
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;

            splitContainer.Panel1.Controls.Add(toolPanel);
            splitContainer.Panel2.Controls.Add(canvas);

            Controls.Add(splitContainer);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var shape in _shapes)
            {
                shape.Draw(e.Graphics);
            }
        }
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _startPos = e.Location;
            _finishPos = e.Location;

            _selectedshape = _shapes.LastOrDefault(shape => shape.ContainsPoint(e.Location));

            if (_selectedshape != null)
            {
                _IsMoving = true;
                foreach (var shape in _shapes)
                {
                    shape.IsSelected = shape == _selectedshape;
                }
                posXNUD.Value = (int)_selectedshape.X;
                posYNUD.Value = (int)_selectedshape.Y;
                angleNUD.Value = (int)_selectedshape.Rotation;
                scaleNUD.Value = (int)_selectedshape.Scale;

            }
            else
            {
                _IsDrawing = true;
            }

            canvas.Invalidate();
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsMoving && _selectedshape != null)
            {
                float deltaX = e.X - _finishPos.X;
                float deltaY = e.Y - _finishPos.Y;

                _selectedshape.X += deltaX;
                _selectedshape.Y += deltaY;

                _finishPos = e.Location;
                canvas.Invalidate();
            }
            else if (_IsDrawing)
            {
                _finishPos = e.Location;
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (_IsDrawing)
            {
                CreateNewShape(e.Location);
            }
            _IsDrawing = false;
            _IsMoving = false;
            _IsRotating = false;
/*надо бы исправить */
            _shapemanager.ExecuteCommand(new MoveShapeCommand(_selectedshape, _startPos, _finishPos));
            posXNUD.Value = (int)_finishPos.X;
            posYNUD.Value = (int)_finishPos.Y;
        }

        private void CreateNewShape(PointF endPoint)
        {
            float width = Math.Abs(endPoint.X - _startPos.X);
            float height = Math.Abs(endPoint.Y - _startPos.Y);

            width = Math.Max(10, width);
            height = Math.Max(10, height);

            Shape newShape = _currentShapeType switch
            {
                ShapeType.Rectangle => new VectorEditor.shapes.Rectangle
                {
                    X = Math.Min(_startPos.X, endPoint.X),
                    Y = Math.Min(_startPos.Y, endPoint.Y),
                    Width = width,
                    Height = height,
                    FillColor = _currentColor
                },
                _ => throw new ArgumentException("Unknown shape type")
            };

            var command = new AddShapeCommand(newShape, _shapes);
            _shapemanager.ExecuteCommand(command);

            _selectedshape = newShape;
            _selectedshape.IsSelected = true;

            canvas.Invalidate();
        }

        private void DelSelectedShape(object sender, EventArgs e)
        {
            if (_selectedshape != null)
            {
                var command = new DeleteShapeCommand(_selectedshape, _shapes);
                _shapemanager.ExecuteCommand(command);
                _selectedshape = null;
            }
        }

        private void BringToFront(Object sender, EventArgs e)
        {
            if (_selectedshape != null)
            {
                int index = _shapes.IndexOf(_selectedshape);
                if (index != _shapes.Count - 1)
                {
                    _shapes[index] = _shapes[index + 1];
                    _shapes[index + 1] = _selectedshape;
                    canvas.Invalidate();
                }
            }
        }
        private void SendToBack(object sender, EventArgs e)
        {
            if (_selectedshape != null)
            {
                int index = _shapes.IndexOf(_selectedshape);
                if (index != 0)
                {
                    _shapes[index] = _shapes[index - 1];
                    _shapes[index - 1] = _selectedshape;
                    canvas.Invalidate();
                }
            }
        }

    }
}
