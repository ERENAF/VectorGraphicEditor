using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;

namespace VectorEditor.commands
{
    public class DeleteShapeCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly List<Shape> _shapes;
        private readonly int _index;

        public DeleteShapeCommand(Shape shape, List<Shape> shapes)
        {
            _shape = shape;
            _shapes = shapes;
            _index = _shapes.IndexOf(_shape);
        }

        public void Execute()
        {
            _shapes.Remove(_shape);
        }
        public void Undo()
        {
            if (_index >= 0 && _index <= _shapes.Count)
            {
                _shapes.Insert(_index, _shape);
            }
            else
            {
                _shapes.Add(_shape);
            }
        }
    }
}
