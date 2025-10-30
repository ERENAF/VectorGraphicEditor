using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEditor.shapes;

namespace VectorEditor.commands
{
    public class ScaleCommand : ICommand
    {
        private readonly float _old_scale;
        private readonly float _new_scale;
        private readonly Shape _shape;

        public ScaleCommand(float old_scale, float new_scale, Shape shape)
        {
            _old_scale = old_scale;
            _new_scale = new_scale;
            _shape = shape;
        }
        public void Execute()
        {
            _shape.Scale = _new_scale;
        }
        public void Undo()
        {
            _shape.Scale = _old_scale;
        }
    }
}
