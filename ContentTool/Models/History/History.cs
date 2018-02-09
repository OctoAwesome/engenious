using System;
using System.Collections.Generic;

namespace ContentTool.Models.History
{
    public class History : IHistory
    {
        public event EventHandler HistoryChanged;
        public event EventHandler HistoryItemAdded;
        private bool _isWorking;
        private readonly Stack<IHistoryItem> _undo, _redo;

        public History()
        {
            _undo = new Stack<IHistoryItem>();
            _redo = new Stack<IHistoryItem>();
        }

        public void Push(IHistoryItem item)
        {
            if (_isWorking)
                return;
            _undo.Push(item);
            HistoryItemAdded?.Invoke(this, EventArgs.Empty);
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        public void Undo()
        {
            var item = _undo.Pop();
            _isWorking = true;
            item.Undo();
            _isWorking = false;
            _redo.Push(item);

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Redo()
        {
            var item = _redo.Pop();
            _isWorking = true;
            item.Redo();
            _isWorking = false;
            _undo.Push(item);

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}