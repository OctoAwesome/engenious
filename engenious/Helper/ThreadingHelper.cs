using System;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace engenious
{
    internal static class Execute
    {
        private static readonly Stack<UiExecutor> FreeList = new Stack<UiExecutor>(16);
        private static readonly object FreeListLock = new object();

        internal class UiExecutor : IDisposable
        {
            internal bool WasOnUiThread;

            public void Dispose()
            {
                if (!WasOnUiThread)
                {
                    try
                    {
                        GL.Flush();

                        try
                        {
                            ThreadingHelper.Context.MakeCurrent(null);
                        }
                        catch (Exception ex) { }
                    }
                    finally
                    {
                        Monitor.Exit(ThreadingHelper.Context);
                    }

                }

                Free(this);
            }
        }

        private static void Free(UiExecutor ex)
        {
            lock (FreeListLock)
            {
                FreeList.Push(ex);
            }
        }

        public static UiExecutor OnUiContext
        {
            get
            {
                UiExecutor ex = null;

                if(FreeList.Count > 0)
                    lock (FreeListLock)
                        if(FreeList.Count > 0)
                            ex = FreeList.Pop();

                ex = ex ?? new UiExecutor();

                if (ThreadingHelper.UiThreadId == Thread.CurrentThread.ManagedThreadId)
                {
                    ex.WasOnUiThread = true;
                    return ex;
                }

                ex.WasOnUiThread = false;
                
                Monitor.Enter(ThreadingHelper.Context);

                try
                {
                    ThreadingHelper.Context.MakeCurrent(ThreadingHelper.WindowInfo);
                }
                catch (Exception e)
                {
                    Monitor.Exit(ThreadingHelper.Context);
                }

                return ex;
            }
        }
    }

    internal static class ThreadingHelper
    {
        private static GLSynchronizationContext sync;
        internal static int UiThreadId;
        private static System.Threading.Thread uiThread;
        internal static IGraphicsContext Context;
        internal static OpenTK.Platform.IWindowInfo WindowInfo;

        static ThreadingHelper()
        {
            uiThread = System.Threading.Thread.CurrentThread;
            UiThreadId = uiThread.ManagedThreadId;
            System.Threading.SynchronizationContext.SetSynchronizationContext(sync = new GLSynchronizationContext());
            //System.Threading.Thread.CurrentThread.Syn
        }

        public static void Initialize(OpenTK.Platform.IWindowInfo windowInfo, int major, int minor, GraphicsContextFlags contextFlags)
        {
            //GraphicsContextFlags flags = GraphicsContextFlags.
            ThreadingHelper.Context = new GraphicsContext(GraphicsMode.Default, windowInfo, major, minor, contextFlags);
            ThreadingHelper.Context.MakeCurrent(windowInfo);
            (ThreadingHelper.Context as IGraphicsContextInternal).LoadAll();
            ThreadingHelper.WindowInfo = windowInfo;
        }

        public static void RunUIThread()
        {
            sync.RunOnCurrentThread();
        }

        public static bool IsOnUIThread()
        {
            return UiThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        internal static void OnUiThread(SendOrPostCallback callback, object obj)
        {
            if (IsOnUIThread())
                callback(obj);
            else
                sync.Post(callback,obj);
        }
    }
}

