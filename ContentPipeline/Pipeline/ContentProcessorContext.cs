using System;
using System.Collections.Generic;
using engenious.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using engenious.Helper;
using engenious.Pipeline;

namespace engenious.Content.Pipeline
{
    public class ContentProcessorContext : ContentContext
    {
        private static INativeWindow Window;

        static ContentProcessorContext()
        {
            
            //BaseWindow = new GameWindow();
            //_window = new NativeWindow(100,100,"Test",GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);

        }

        public ContentProcessorContext(SynchronizationContext syncContext, string workingDirectory = "")
        {
            SyncContext = syncContext;
            WorkingDirectory = workingDirectory;
            
            var window = new GameWindow(100, 100);
            ContentProcessorContext.Window = window;
            var windowInfo = window.WindowInfo;
            var context = window.Context;

            context.MakeCurrent(windowInfo);
            (context as IGraphicsContextInternal)?.LoadAll();

            ThreadingHelper.Initialize(null, null, 0, 0, GraphicsContextFlags.Debug);

            GraphicsDevice = new GraphicsDevice(null, ThreadingHelper.Context);
        }

        public SynchronizationContext SyncContext { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public string WorkingDirectory { get; private set; }
        public List<SourceFile> SourceFiles { get; set; }

        public override void Dispose()
        {
            //GraphicsDevice.Dispose();
            //Window.Dispose();
        }
    }
}