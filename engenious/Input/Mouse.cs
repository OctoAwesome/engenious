using System;
using System.Reflection;
using engenious.Helper;
using OpenTK;
using OpenTK.Input;

namespace engenious.Input
{
    public static class Mouse
    {

        private static GameWindow _window;
        private static float _deltaPrecise;
        static Mouse()
        {
            if (!WrappingHelper.ValidateStructs<OpenTK.Input.MouseState,MouseState>())
                throw new Exception("test");
        }

        internal static void UpdateWindow(GameWindow window)
        {
            _window = window;
            window.MouseWheel += delegate(object sender, MouseWheelEventArgs e)
            {
                _deltaPrecise-=(e.DeltaPrecise);
            };
        }
        public static MouseState GetState(int index)
        {
            return GetState();
            //TODO multiple mice
            /*OpenTK.Input.MouseState state = OpenTK.Input.Mouse.GetState(index);
            MouseState actual = *(MouseState*)(&state);
            
            actual.X = _window.Mouse.X;
            actual.Y = _window.Mouse.Y;
            return actual;*/

        }
        public static unsafe MouseState GetState()
        {
            var state = OpenTK.Input.Mouse.GetState();
            var actual = *(MouseState*)(&state);
            actual.Scroll = new MouseScroll(actual.Scroll.X,_deltaPrecise);//.Y = ;
            //deltaPrecise = 0;
            return actual;

        }

        public static void SetPosition(double x, double y)
        {
            var pt = _window.PointToScreen(new System.Drawing.Point((int)x, (int)y));
            x = pt.X + (x - (int)x);
            y = pt.Y + (x - (int)y);
            OpenTK.Input.Mouse.SetPosition(x,y);
        }
    }
}

