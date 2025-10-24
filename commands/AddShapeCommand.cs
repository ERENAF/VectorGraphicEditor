using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;
namespace VectorEditor.commands
{
    public class AddShapeCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly List<Shape> _shapes;

        public AddShapeCommand(Shape shape, List<Shape> shapes)
        {
            _shape = shape;
            _shapes = shapes;
        }
        public void Execute()
        {
            _shapes.Add(_shape);
        }
        public void Undo()
        {
            _shapes.Remove(_shape);
        }
    }
}
