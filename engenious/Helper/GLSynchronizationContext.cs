using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace engenious.Helper
{
    internal sealed class GlSynchronizationContext : SynchronizationContext
    {
        private Thread _currentThread;

        private class CallbackState
        {
            public CallbackState()
            {
                WaitHandle = new AutoResetEvent(false);
                DoWait = false;
            }
            public SendOrPostCallback Callback;
            public object UserState;
            public AutoResetEvent WaitHandle;
            public bool DoWait;
        }

        private readonly object _syncRoot = new object();
        
        private int _count,_head,_tail;
        private readonly List<CallbackState> _array = new List<CallbackState>();

        public GlSynchronizationContext()
        {
            for (int i = 0; i < 128; i++)
            {
                _array.Add(new CallbackState());
            }
        }
        
        public override void Send(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread == _currentThread)
            {
                callback(state);
            }
            else
            {
                Add(callback,state,true).WaitOne();
            }
        }
        public override void Post(SendOrPostCallback callback, object state)
        {
            Add(callback, state);
        }

        private AutoResetEvent Add(SendOrPostCallback callback, object state,bool wait=false)
        {
            lock (_syncRoot)
            {
                if (_count == _array.Count) {
                    int newcapacity = (int)((long)_array.Count * 2);
                    if (newcapacity < _array.Count + 4) {
                        newcapacity = _array.Count + 4;
                    }

                    int added = newcapacity - _array.Capacity;
                    _array.Capacity = newcapacity;
                    for (int i = 0; i < added; i++)
                    {
                        _array.Add(new CallbackState());
                    }
                }
                var item = _array[_tail];
                item.Callback = callback;
                item.UserState = state;
                item.DoWait = wait;
                _tail = (_tail + 1) % _array.Count;
                _count++;

                return item.WaitHandle;
            }
        }

        private bool TryTake(ref CallbackState callbackState)
        {
            if (_count == 0)
                return false;
            lock (_syncRoot)
            {
                if (_count == 0)
                    return false;
                callbackState = _array[_head];
                _head = (_head + 1) % _array.Count;
                _count--;
                return true;
            }
        }
        public void RunOnCurrentThread()
        {
            _currentThread = Thread.CurrentThread;
            var workItem = default(CallbackState);
            while (TryTake(ref workItem))
            {
                var callbackState = workItem;

                callbackState.Callback(callbackState.UserState);
                if(callbackState.DoWait)
                    callbackState.WaitHandle.Set();
            }

        }
    }
}

