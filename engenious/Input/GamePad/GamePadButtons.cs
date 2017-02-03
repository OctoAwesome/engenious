using System;

namespace engenious.Input
{
    public struct GamePadButtons : IEquatable<GamePadButtons>
    {
        private readonly Buttons _buttons;
        //
        // Properties
        //
        public ButtonState A => GetButton(Buttons.A);

        public ButtonState B => GetButton(Buttons.B);

        public ButtonState Back => GetButton(Buttons.Back);

        public ButtonState BigButton => GetButton(Buttons.Home);

        public ButtonState LeftShoulder => GetButton(Buttons.LeftShoulder);

        public ButtonState LeftStick => GetButton(Buttons.LeftStick);

        public ButtonState RightShoulder => GetButton(Buttons.RightShoulder);

        public ButtonState RightStick => GetButton(Buttons.RightStick);

        public ButtonState Start => GetButton(Buttons.Start);

        public ButtonState X => GetButton(Buttons.X);

        public ButtonState Y => GetButton(Buttons.Y);

        //
        // Constructors
        //
        public GamePadButtons(Buttons state)
        {
            _buttons = state;
        }

        //
        // Methods
        //
        public bool Equals(GamePadButtons other)
        {
            return _buttons == other._buttons;
        }

        public override bool Equals(object obj)
        {
            return obj is GamePadButtons && Equals((GamePadButtons)obj);
        }

        private ButtonState GetButton(Buttons b)
        {
            return (_buttons & b) == 0 ? ButtonState.Released : ButtonState.Pressed;
        }

        public override int GetHashCode()
        {
            return _buttons.GetHashCode();
        }

        public override string ToString()
        {
            return Convert.ToString((int)_buttons, 2).PadLeft(10, '0');
        }

        //
        // Operators
        //
        public static bool operator ==(GamePadButtons left, GamePadButtons right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GamePadButtons left, GamePadButtons right)
        {
            return !left.Equals(right);
        }
    }
}

