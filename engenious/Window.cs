using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace engenious
{
    public class Window : IDisposable
    {
        internal GameWindow BaseWindow;
        internal Window(GameWindow baseWindow)
        {
            BaseWindow = baseWindow;
            baseWindow.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.F4 && e.Alt)
                {
                    Close();
                }
            };

        }

        public Point PointToScreen(Point pt)
        {
            return BaseWindow.PointToScreen(new System.Drawing.Point(pt.X,pt.Y));
        }
        public Point PointToClient(Point pt)
        {
            return BaseWindow.PointToClient(new System.Drawing.Point(pt.X,pt.Y));
        }
        
        public Rectangle ClientRectangle{
            get{
                return new Rectangle(BaseWindow.ClientRectangle.X,BaseWindow.ClientRectangle.Y,BaseWindow.ClientRectangle.Width,BaseWindow.ClientRectangle.Height);
            }
            set{
                BaseWindow.ClientRectangle = new System.Drawing.Rectangle(value.X,value.Y,value.Width,value.Height);
            }
        }
        public Size ClientSize{
            get{
                return new Size(BaseWindow.ClientSize.Width,BaseWindow.ClientSize.Height);
            }
            set{
                BaseWindow.ClientSize = new System.Drawing.Size(value.Width,value.Height);
            }
        }
        public Icon Icon { get { return BaseWindow.Icon; } set { BaseWindow.Icon = value; } }
        public bool Visible{
            get{
                return BaseWindow.Visible;
            }
            set{
                BaseWindow.Visible = value;
            }
        }
        public bool Focused => BaseWindow.Focused;

        public bool CursorVisible{
            get{
                return BaseWindow.CursorVisible;
            }
            set{
                BaseWindow.CursorVisible = value;
            }
        }
        public bool IsBorderless{
            get{
                return BaseWindow.WindowBorder != WindowBorder.Hidden;
            }
            set{
                if (value)
                    BaseWindow.WindowBorder = WindowBorder.Hidden;
                else
                    BaseWindow.WindowBorder = _allowUserResizing ? WindowBorder.Resizable : WindowBorder.Fixed;
            }
        }
        private bool _allowUserResizing;
        public bool AllowUserResizing{
            get{
                return _allowUserResizing;
            }
            set{
                _allowUserResizing = value;
                if (IsBorderless)
                    return;
                BaseWindow.WindowBorder = _allowUserResizing ? WindowBorder.Resizable : WindowBorder.Fixed;
            }
        }
        private bool _fullscreen;
        private WindowState _oWindowState= WindowState.Normal;
        public bool Fullscreen{
            get{
                return _fullscreen;
            }
            set{
                if (!_fullscreen)
                    _oWindowState = BaseWindow.WindowState;
                _fullscreen = value;
                BaseWindow.WindowState = _fullscreen ? WindowState.Fullscreen : _oWindowState;
            }
        }
        public string Title{
            get{
                return BaseWindow.Title;
            }
            set{
                BaseWindow.Title = value;
            }
        }
        public Point Position{
            get{
                return new Point(BaseWindow.X,BaseWindow.Y);
            }
            set{
                X = value.X;
                Y = value.Y;
            }
        }
        public int X{
            get{
                return BaseWindow.X;
            }
            set{
                BaseWindow.X = value;
            }
        }
        public int Y{
            get{
                return BaseWindow.Y;
            }
            set{
                BaseWindow.Y = value;
            }
        }



        public void Close()
        {
            BaseWindow.Close();
        }

        #region IDisposable implementation

        public void Dispose()
        {
            BaseWindow.Dispose();
        }

        #endregion
    }
}

