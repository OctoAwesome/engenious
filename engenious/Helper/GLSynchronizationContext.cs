using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace engenious
{
    sealed class GLSynchronizationContext : SynchronizationContext
    {
        private Thread currentThread;
        class CallbackState
        {
            public CallbackState(object state, ManualResetEvent waitHandle)
            {
                UserState = state;
                WaitHandle = waitHandle;
            }
            public object UserState;
            public ManualResetEvent WaitHandle;
        }
        BlockingCollection<KeyValuePair<SendOrPostCallback, CallbackState>> queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, CallbackState>>();

        public override void Send(SendOrPostCallback d, object state)
        {
            if (Thread.CurrentThread == currentThread)
            {
                d(state);
            }
            else
            {
                var waitHandle = new ManualResetEvent(false);
                queue.Add(new KeyValuePair<SendOrPostCallback, CallbackState>(d, new CallbackState(state, waitHandle)));
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }
        }
        public override void Post(SendOrPostCallback d, object state)
        {
            queue.Add(new KeyValuePair<SendOrPostCallback, CallbackState>(d, new CallbackState(state,null)));
        }

        public void RunOnCurrentThread()
        {
            currentThread = Thread.CurrentThread;
            KeyValuePair<SendOrPostCallback, CallbackState> workItem;

            while (queue.Count > 0 && queue.TryTake(out workItem, Timeout.Infinite))
            {
                var callbackState = workItem.Value;
                var waitHandle = callbackState.WaitHandle;

                workItem.Key(callbackState.UserState);
                waitHandle?.Set();
            }

        }
    }
}

