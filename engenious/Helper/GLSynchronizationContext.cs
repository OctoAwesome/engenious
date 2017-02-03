using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace engenious
{
    internal sealed class GlSynchronizationContext : SynchronizationContext
    {
        private Thread _currentThread;

        private class CallbackState
        {
            public CallbackState(object state, ManualResetEvent waitHandle)
            {
                UserState = state;
                WaitHandle = waitHandle;
            }
            public readonly object UserState;
            public readonly ManualResetEvent WaitHandle;
        }

        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, CallbackState>> _queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, CallbackState>>();

        public override void Send(SendOrPostCallback d, object state)
        {
            if (Thread.CurrentThread == _currentThread)
            {
                d(state);
            }
            else
            {
                var waitHandle = new ManualResetEvent(false);//TODO: memory pooling
                _queue.Add(new KeyValuePair<SendOrPostCallback, CallbackState>(d, new CallbackState(state, waitHandle)));//TODO: memory pooling
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }
        }
        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Add(new KeyValuePair<SendOrPostCallback, CallbackState>(d, new CallbackState(state,null)));//TODO: memory pooling
        }

        public void RunOnCurrentThread()
        {
            _currentThread = Thread.CurrentThread;
            KeyValuePair<SendOrPostCallback, CallbackState> workItem;

            while (_queue.Count > 0 && _queue.TryTake(out workItem, Timeout.Infinite))
            {
                var callbackState = workItem.Value;
                var waitHandle = callbackState.WaitHandle;

                workItem.Key(callbackState.UserState);
                waitHandle?.Set();
            }

        }
    }
}

