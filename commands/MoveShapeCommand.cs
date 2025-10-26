using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;

namespace VectorEditor.commands
{
    public class MoveShapeCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly PointF _oldPos;
        private readonly PointF _newPos;

        public MoveShapeCommand(Shape shape, PointF oldPos, PointF newPos)
        {
            _shape = shape;
            _oldPos = oldPos;
            _newPos = newPos;
        }
        public void Execute()
        {
            _shape.X = _newPos.X;
            _shape.Y = _newPos.Y;
        }
        public void Undo()
        {
            _shape.X = _oldPos.X;
            _shape.Y= _oldPos.Y;
        }

    }
}
