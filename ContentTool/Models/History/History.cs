using System;
using System.Collections.Generic;

namespace ContentTool.Models.History
{
    public class History : IHistoryItem
    {
        public event EventHandler HistoryChanged; 
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
            HistoryChanged?.Invoke(this,EventArgs.Empty);
        }

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        public void Undo()
        {
            IHistoryItem item = _undo.Pop();
            _isWorking = true;
            item.Undo();
            _isWorking = false;
            _redo.Push(item);
            
            HistoryChanged?.Invoke(this,EventArgs.Empty);
        }

        public void Redo()
        {
            IHistoryItem item = _redo.Pop();
            _isWorking = true;
            item.Redo();
            _isWorking = false;
            _undo.Push(item);
            
            HistoryChanged?.Invoke(this,EventArgs.Empty);
        }
    }
}