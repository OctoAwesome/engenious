using System;
using engenious.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace engenious.Content.Pipeline
{
    public class ContentProcessorContext: ContentContext
    {
        private INativeWindow window;
        private IGraphicsContext context;
        public ContentProcessorContext(string workingDirectory = "")
        {
            this.WorkingDirectory = workingDirectory;
            //window = new GameWindow();
            window = new NativeWindow();

            ThreadingHelper.Initialize(window.WindowInfo, 3, 1, GraphicsContextFlags.Debug);
            GraphicsDevice = new GraphicsDevice(null, ThreadingHelper.Context);

        }

        public GraphicsDevice GraphicsDevice{ get; private set; }

        public string WorkingDirectory{ get; private set; }

        public override void Dispose()
        {
            //GraphicsDevice.Dispose();
            //window.Dispose();
        }
    }
}