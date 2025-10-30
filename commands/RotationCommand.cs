using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;

namespace VectorEditor.commands
{
    public class RotationCommand: ICommand
    {
        private readonly Shape _shape;
        private readonly float _old_rotation;
        private readonly float _new_rotation;

        public RotationCommand(Shape shape, float new_rotation)
        {
            _shape = shape;
            _old_rotation = shape.Rotation;
            _new_rotation = new_rotation;
        }

        public void Execute()
        {
            _shape.Rotation = _new_rotation;
        }
        public void Undo()
        {
            _shape.Rotation -= _old_rotation;
        }
    }
}
