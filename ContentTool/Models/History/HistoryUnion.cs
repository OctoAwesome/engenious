using System;
using System.Collections.Generic;

namespace ContentTool.Models.History
{
    public class HistoryUnion : IHistory
    {
        public event EventHandler HistoryChanged;
        public event EventHandler HistoryItemAdded;

        private bool _isWorking;
        private readonly List<IHistory> _histories;
        private Stack<IHistory> _undo, _redo;

        public HistoryUnion()
        {
            _histories = new List<IHistory>();
            _undo = new Stack<IHistory>();
            _redo = new Stack<IHistory>();
        }

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


        public void Push(IHistoryItem item)
        {
            throw new NotSupportedException();
        }

        public void Add(IHistory history)
        {
            _histories.Add(history);
            history.HistoryItemAdded += HistoryOnHistoryItemAdded;
        }

        public void Remove(IHistory history)
        {
            history.HistoryItemAdded -= HistoryOnHistoryItemAdded;
            _histories.Remove(history);


            RemoveFromStack(ref _undo, history);
            RemoveFromStack(ref _redo, history);
        }

        private static void RemoveFromStack<T>(ref Stack<T> stack, T remove) where T : class
        {
            var tmp = new Stack<T>();
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (item != remove)
                    tmp.Push(item);
            }
            stack = tmp;
        }

        private void HistoryOnHistoryItemAdded(object sender, EventArgs eventArgs)
        {
            if (_isWorking)
                return;
            var history = (IHistory) sender;
            _undo.Push(history);
        }

        public bool CanUndo => _undo.Count > 0 && _undo.Peek().CanUndo;
        public bool CanRedo => _redo.Count > 0 && _redo.Peek().CanRedo;
    }
}