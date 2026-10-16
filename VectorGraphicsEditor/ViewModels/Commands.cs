using System;
using System.Windows.Input;

namespace GraphicsApp.ViewModels
{
    public interface IUndoCommand
    {
        void Execute();
        void Undo();
    }

    public class PropertyReference<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public PropertyReference(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public T Value
        {
            get => _getter();
            set => _setter(value);
        }
    }
}