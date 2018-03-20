#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;

namespace engenious.Input
{
    /// <summary>
    /// Encapsulates the state of a mouse device.
    /// </summary>
    public struct MouseState : IEquatable<MouseState>
    {
        #region Fields
        internal const int WheelDelta = 1;//Windows wheel
        internal const int MaxButtons = 16;
        // we are storing in an ushort
        private Vector2 _position;

        private MouseScroll _scroll;
        private ushort _buttons;
        private bool _isConnected;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the specified
        /// <see cref="OpenTK.Input.MouseButton"/> is pressed.
        /// </summary>
        /// <param name="button">The <see cref="OpenTK.Input.MouseButton"/> to check.</param>
        /// <returns>True if key is pressed; false otherwise.</returns>
        public bool this [MouseButton button]
        {
            get { return IsButtonDown(button); }
            internal set
            {
                if (value)
                    EnableBit((int)button);
                else
                    DisableBit((int)button);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this button is down.
        /// </summary>
        /// <param name="button">The <see cref="OpenTK.Input.MouseButton"/> to check.</param>
        public bool IsButtonDown(MouseButton button)
        {
            return ReadBit((int)button);
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this button is up.
        /// </summary>
        /// <param name="button">The <see cref="OpenTK.Input.MouseButton"/> to check.</param>
        public bool IsButtonUp(MouseButton button)
        {
            return !ReadBit((int)button);
        }

        /// <summary>
        /// Gets the absolute wheel position in integer units.
        /// To support high-precision mice, it is recommended to use <see cref="WheelPrecise"/> instead.
        /// </summary>
        public int Wheel
        {
            get { return (int)(_scroll.Y*WheelDelta); }
        }

        /// <summary>
        /// Gets the absolute wheel position in floating-point units.
        /// </summary>
        public float WheelPrecise
        {
            get { return _scroll.Y; }
        }

        /// <summary>
        /// Gets a <see cref="OpenTK.Input.MouseScroll"/> instance,
        /// representing the current state of the mouse scroll wheel.
        /// </summary>
        public MouseScroll Scroll
        {
            get { return _scroll; }
            internal set{_scroll = value;}
        }

        /// <summary>
        /// Gets an integer representing the absolute x position of the pointer, in BaseWindow pixel coordinates.
        /// </summary>
        public int X
        {
            get { return (int)Math.Round(_position.X); }
            internal set { _position.X = value; }
        }

        /// <summary>
        /// Gets an integer representing the absolute y position of the pointer, in BaseWindow pixel coordinates.
        /// </summary>
        public int Y
        {
            get { return (int)Math.Round(_position.Y); }
            internal set { _position.Y = value; }
        }
        /// <summary>
        /// Gets an integer representing the absolute x,y position of the pointer, in BaseWindow pixel coordinates.
        /// </summary>
        public Point Location => new Point(X, Y);
        
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the left mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public ButtonState LeftButton
        {
            get { return IsButtonDown(MouseButton.Left) ? ButtonState.Pressed : ButtonState.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the middle mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public ButtonState MiddleButton
        {
            get { return IsButtonDown(MouseButton.Middle) ? ButtonState.Pressed : ButtonState.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the right mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public ButtonState RightButton
        {
            get { return IsButtonDown(MouseButton.Right) ? ButtonState.Pressed : ButtonState.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the first extra mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public ButtonState XButton1
        {
            get { return IsButtonDown(MouseButton.Button1) ? ButtonState.Pressed : ButtonState.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the second extra mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public ButtonState XButton2
        {
            get { return IsButtonDown(MouseButton.Button2) ? ButtonState.Pressed : ButtonState.Released; }
        }

        /// <summary>
        /// Gets a value indicating whether any button is down.
        /// </summary>
        /// <value><c>true</c> if any button is down; otherwise, <c>false</c>.</value>
        public bool IsAnyButtonDown
        {
            get
            {
                // If any bit is set then a button is down.
                return _buttons != 0;
            }
        }

        /// <summary>
        /// Gets the absolute wheel position in integer units. This property is intended for XNA compatibility.
        /// To support high-precision mice, it is recommended to use <see cref="WheelPrecise"/> instead.
        /// </summary>
        public int ScrollWheelValue
        {
            get { return Wheel; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return _isConnected; }
            internal set { _isConnected = value; }
        }

        /// <summary>
        /// Checks whether two <see cref="MouseState" /> instances are equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is equal to right; false otherwise.
        /// </returns>
        public static bool operator ==(MouseState left, MouseState right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="MouseState" /> instances are not equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is not equal to right; false otherwise.
        /// </returns>
        public static bool operator !=(MouseState left, MouseState right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares to an object instance for equality.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare to.
        /// </param>
        /// <returns>
        /// True if this instance is equal to obj; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is MouseState)
            {
                return this == (MouseState)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a hashcode for the current instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/> represting the hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return _buttons.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ _scroll.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.MouseState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.MouseState"/>.</returns>
        public override string ToString()
        {
            var b = Convert.ToString(_buttons, 2).PadLeft(10, '0');
            return $"[X={X}, Y={Y}, Scroll={Scroll}, Buttons={b}, IsConnected={IsConnected}]";
        }

        #endregion

        #region Internal Members

        internal Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        internal bool ReadBit(int offset)
        {
            ValidateOffset(offset);
            return (_buttons & (1 << offset)) != 0;
        }

        internal void EnableBit(int offset)
        {
            ValidateOffset(offset);
            _buttons |= unchecked((ushort)(1 << offset));
        }

        internal void DisableBit(int offset)
        {
            ValidateOffset(offset);
            _buttons &= unchecked((ushort)(~(1 << offset)));
        }

        internal void MergeBits(MouseState other)
        {
            _buttons |= other._buttons;
            SetScrollRelative(other._scroll.X, other._scroll.Y);
            X += other.X;
            Y += other.Y;
            IsConnected |= other.IsConnected;
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        #region Internal Members

        internal void SetScrollAbsolute(float x, float y)
        {
            _scroll.X = x;
            _scroll.Y = y;
        }

        internal void SetScrollRelative(float x, float y)
        {
            _scroll.X += x;
            _scroll.Y += y;
        }

        #endregion

        #endregion

        #region Private Members

        static void ValidateOffset(int offset)
        {
            if (offset < 0 || offset >= 16)
                throw new ArgumentOutOfRangeException("offset");
        }

        #endregion

        #region IEquatable<MouseState> Members

        /// <summary>
        /// Compares two MouseState instances.
        /// </summary>
        /// <param name="other">The instance to compare two.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(MouseState other)
        {
            return
                _buttons == other._buttons &&
            X == other.X &&
            Y == other.Y &&
            Scroll == other.Scroll;
        }

        #endregion
    }
}