using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using engenious.Graphics;
using engenious.Content;
using System.Linq;
using engenious.Audio;
using engenious.Input;
using OpenTK.Audio.OpenAL;

namespace engenious
{
    public delegate void KeyPressDelegate(object sender,char Key);
    public abstract class Game : GraphicsResource
    {
        public event KeyPressDelegate KeyPress;
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler Exiting;
        public event EventHandler Resized;

        private GameTime gameTime;
        internal int major, minor;
        internal OpenTK.Graphics.GraphicsContextFlags contextFlags;
        private OpenTK.Graphics.IGraphicsContext Context;
        private Audio.AudioDevice audio;
        private void ConstructContext()
        {
            OpenTK.Platform.IWindowInfo windowInfo = Window.window.WindowInfo;
            if (Window.window.Context == null)
            {//TODO: really - what was that even for?
                OpenTK.Graphics.GraphicsContextFlags flags = OpenTK.Graphics.GraphicsContextFlags.Default;
                int major = 1;
                int minor = 0;

                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT ||
                    System.Environment.OSVersion.Platform == PlatformID.Win32S ||
                    System.Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                    System.Environment.OSVersion.Platform == PlatformID.WinCE)
                {
                    major = 4;
                    minor = 4;
                }
                if (this.Context == null || this.Context.IsDisposed)
                {

                    OpenTK.Graphics.ColorFormat colorFormat = new OpenTK.Graphics.ColorFormat(8, 8, 8, 8);
                    int depth = 24; //TODO: wth?
                    int stencil = 8;
                    int samples = 4;

                    OpenTK.Graphics.GraphicsMode mode =
                        new OpenTK.Graphics.GraphicsMode(colorFormat, depth, stencil, samples);
                    try
                    {
                        this.Context = new OpenTK.Graphics.GraphicsContext(mode, windowInfo, major, minor, flags);
                        //this.Context = Window.Context;
                    }
                    catch (Exception ex)
                    {
                        mode = OpenTK.Graphics.GraphicsMode.Default;
                        major = 1;
                        minor = 0;
                        flags = OpenTK.Graphics.GraphicsContextFlags.Default;
                        this.Context = new OpenTK.Graphics.GraphicsContext(mode, windowInfo, major, minor, flags);
                    }
                }
            }
            else
                this.Context = Window.window.Context;
            this.Context.MakeCurrent(windowInfo);
            (this.Context as OpenTK.Graphics.IGraphicsContextInternal).LoadAll();
            ThreadingHelper.Initialize(windowInfo, major, minor, contextFlags);
            this.Context.MakeCurrent(windowInfo);

        }

        public Game()
        {
            audio = new AudioDevice();
            OpenTK.Graphics.GraphicsContext.ShareContexts = true;
            var window = new GameWindow(1280, 720);

            Window = new Window(window);
            ConstructContext();

            GraphicsDevice = new GraphicsDevice(this, Context);
            GraphicsDevice.Viewport = new Viewport(window.ClientRectangle);
            window.Context.MakeCurrent(window.WindowInfo);
            window.Context.LoadAll();
            GL.Viewport(window.ClientRectangle);
            //Window.Location = new System.Drawing.Point();
            Mouse = new MouseDevice(window.Mouse);
            engenious.Input.Mouse.UpdateWindow(window);
            window.FocusedChanged += Window_FocusedChanged;
            window.Closing += delegate(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Exiting?.Invoke(this, new EventArgs());
            };
            
            gameTime = new GameTime(new TimeSpan(), new TimeSpan());

            window.UpdateFrame += delegate(object sender, FrameEventArgs e)
            {
                Components.Sort();

                gameTime.Update(e.Time);

                Update(gameTime);
            };
            window.RenderFrame += delegate(object sender, FrameEventArgs e)
            {
                ThreadingHelper.RunUiThread();
                GraphicsDevice.Clear(Color.CornflowerBlue);
                Draw(gameTime);

                GraphicsDevice.Present();
            };
            window.Resize += delegate(object sender, EventArgs e)
            {
                GraphicsDevice.Viewport = new Viewport(window.ClientRectangle);
                GL.Viewport(window.ClientRectangle);

                Resized?.Invoke(sender,e);
                OnResize(this, e);
            };
            window.Load += delegate(object sender, EventArgs e)
            {
                Initialize();
                LoadContent();
            };
            window.Closing += delegate(object sender, System.ComponentModel.CancelEventArgs e)
            {
                OnExiting(this, new EventArgs());
            };
            window.KeyPress += delegate(object sender, KeyPressEventArgs e)
            {
                KeyPress?.Invoke(this, e.KeyChar);
            };

            //Window.Context.MakeCurrent(Window.WindowInfo);

            Content = new engenious.Content.ContentManager(GraphicsDevice);
            Components = new GameComponentCollection();
           

        }

        void Window_FocusedChanged(object sender, EventArgs e)
        {
            if (Window.Focused)
                Activated?.Invoke(this, e);
            else
                Deactivated?.Invoke(this, e);
        }

        protected virtual void Initialize()
        {
            Window.Visible = true;
            GL.Enable(EnableCap.Blend);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //GraphicsDevice.SamplerStates = new SamplerStateCollection(GraphicsDevice);

            foreach (var component in Components)
            {
                component.Initialize();
            }
        }

        public void Exit()
        {
            Window.Close();
        }

        protected virtual void OnResize(object sender, EventArgs e)
        {
        }

        protected virtual void OnExiting(object sender, EventArgs e)
        {
        }

        public ContentManager Content{ get; private set; }

        public System.Drawing.Icon Icon { get { return Window.Icon; } set { Window.Icon = value; } }
        
        public Window Window{ get; private set; }

        public MouseDevice Mouse{ get; private set; }

        public string Title{ get { return Window.Title; } set { Window.Title = value; } }

        public bool IsMouseVisible
        {
            get
            {
                return Window.CursorVisible;
            }
            set
            {
                Window.CursorVisible = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return Window.Focused;
            }
        }

        public void Run()
        {
            Window.window.Run();
        }

        public void Run(double updatesPerSec, double framesPerSec)
        {
            Window.window.Run(updatesPerSec, framesPerSec);
        }

        public GameComponentCollection Components{ get; private set; }

        public virtual void LoadContent()
        {
            foreach (var component in Components)
            {
                component.Load();
            }
        }

        public virtual void UnloadContent()
        {
            foreach (var component in Components)
            {
                component.Unload();
            }
        }

        public virtual void Update(GameTime gameTime)
        {

            foreach (var component in Components.Updatables)
            {
                if (!component.Enabled)
                    break;
                component.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (var component in Components.Drawables)
            {
                if (!component.Visible)
                    break;
                component.Draw(gameTime);
            }
        }

        #region IDisposable

        public override void Dispose()
        {
            Window.Dispose();

            base.Dispose();
        }

        #endregion
    }
}

