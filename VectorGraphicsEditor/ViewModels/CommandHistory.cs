using System.Collections.Generic;

namespace GraphicsApp.ViewModels
{
    public class CommandHistory
    {
        private readonly Stack<IUndoCommand> _undoStack = new();
        private readonly Stack<IUndoCommand> _redoStack = new();

        public void Add(IUndoCommand command)
        {
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Execute(IUndoCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public IUndoCommand Pop()
        {
            return _undoStack.Pop();
        }
        public void Push(IUndoCommand command)
        {
            _undoStack.Push(command);
        }

        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
        }
    }
}