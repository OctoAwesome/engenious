using System;

namespace engenious.Input
{
    public struct GamePadDPad : IEquatable<GamePadDPad>
    {
        private DPadButtons _buttons;
        //
        // Properties
        //
        public ButtonState Down => !IsDown ? ButtonState.Released : ButtonState.Pressed;

        public bool IsDown
        {
            get
            {
                return (byte)(_buttons & DPadButtons.Down) != 0;
            }
            internal set
            {
                SetButton(DPadButtons.Down, value);
            }
        }

        public bool IsLeft
        {
            get
            {
                return (byte)(_buttons & DPadButtons.Left) != 0;
            }
            internal set
            {
                SetButton(DPadButtons.Left, value);
            }
        }

        public bool IsRight
        {
            get
            {
                return (byte)(_buttons & DPadButtons.Right) != 0;
            }
            internal set
            {
                SetButton(DPadButtons.Right, value);
            }
        }

        public bool IsUp
        {
            get
            {
                return (byte)(_buttons & DPadButtons.Up) != 0;
            }
            internal set
            {
                SetButton(DPadButtons.Up, value);
            }
        }

        public ButtonState Left => !IsLeft ? ButtonState.Released : ButtonState.Pressed;

        public ButtonState Right => !IsRight ? ButtonState.Released : ButtonState.Pressed;

        public ButtonState Up => !IsUp ? ButtonState.Released : ButtonState.Pressed;

        //
        // Constructors
        //
        internal GamePadDPad(Buttons state)
        {
            _buttons = (DPadButtons)(state & (Buttons.DPadUp | Buttons.DPadDown | Buttons.DPadLeft | Buttons.DPadRight));
        }

        //
        // Methods
        //
        public override bool Equals(object obj)
        {
            return obj is GamePadDPad && Equals((GamePadDPad)obj);
        }

        public bool Equals(GamePadDPad other)
        {
            return _buttons == other._buttons;
        }

        public override int GetHashCode()
        {
            return _buttons.GetHashCode();
        }

        private void SetButton(DPadButtons button, bool value)
        {
            if (value)
            {
                _buttons |= button;
                return;
            }
            _buttons &= ~button;
        }

        public override string ToString()
        {
            return string.Format("{{{0}{1}{2}{3}}}", IsUp ? "U" : string.Empty, IsLeft ? "L" : string.Empty, IsDown ? "D" : string.Empty, IsRight ? "R" : string.Empty);
        }

        //
        // Operators
        //
        public static bool operator ==(GamePadDPad left, GamePadDPad right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GamePadDPad left, GamePadDPad right)
        {
            return !left.Equals(right);
        }

        //
        // Nested Types
        //
        [Flags]
        private enum DPadButtons : byte
        {
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8
        }
    }
}

