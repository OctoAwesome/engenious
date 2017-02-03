using System;
using engenious.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using System.ComponentModel;

namespace engenious.Content.Pipeline
{
    public class ContentProcessorContext: ContentContext
    {
        private readonly INativeWindow _window;

        public ContentProcessorContext(SynchronizationContext syncContext,string workingDirectory = "")
        {
            SyncContext = syncContext;
            WorkingDirectory = workingDirectory;
            //BaseWindow = new GameWindow();
            _window = new NativeWindow();

            ThreadingHelper.Initialize(_window.WindowInfo, 3, 1, GraphicsContextFlags.Debug);
            GraphicsDevice = new GraphicsDevice(null, ThreadingHelper.Context);

        }

        public SynchronizationContext SyncContext { get; private set; }
        public GraphicsDevice GraphicsDevice{ get; private set; }

        public string WorkingDirectory{ get; private set; }

        public override void Dispose()
        {
            //GraphicsDevice.Dispose();
            _window.Dispose();
        }
    }
}