using System;

namespace engenious.Input
{
    public struct GamePadState : IEquatable<GamePadState>
    {
        private const float RangeMultiplier = 1.0f / (short.MaxValue + 1);

        private Buttons _buttons;
        private int _packetNumber;
        private short _leftStickX;
        private short _leftStickY;
        private short _rightStickX;
        private short _rightStickY;
        private byte _leftTrigger;
        private byte _rightTrigger;
        private bool _isConnected;
        //
        // Properties
        //
        public GamePadButtons Buttons => new GamePadButtons(_buttons);

        public GamePadDPad DPad => new GamePadDPad(_buttons);

        public bool IsConnected => _isConnected;

        public int PacketNumber => _packetNumber;

        public GamePadThumbSticks ThumbSticks => new GamePadThumbSticks(_leftStickX, _leftStickY, _rightStickX, _rightStickY);

        public GamePadTriggers Triggers => new GamePadTriggers(_leftTrigger, _rightTrigger);

        //
        // Methods
        //
        public bool Equals(GamePadState other)
        {
            return ThumbSticks == other.ThumbSticks && Buttons == other.Buttons && DPad == other.DPad && IsConnected == other.IsConnected;
        }

        public override bool Equals(object obj)
        {
            return obj is GamePadState && Equals((GamePadState)obj);
        }

        public override int GetHashCode()
        {
            return ThumbSticks.GetHashCode() ^ Buttons.GetHashCode() ^ DPad.GetHashCode() ^ IsConnected.GetHashCode();
        }

        private bool IsAxisValid(GamePadAxes axis)
        {
            return axis >= 0 && axis < (GamePadAxes.LeftY | GamePadAxes.RightX);
        }

        private bool IsDPadValid(int index)
        {
            return index >= 0 && index < 2;
        }

        internal void SetAxis(GamePadAxes axis, short value)
        {
            if ((byte)(axis & GamePadAxes.LeftX) != 0)
            {
                _leftStickX = value;
            }
            if ((byte)(axis & GamePadAxes.LeftY) != 0)
            {
                _leftStickY = value;
            }
            if ((byte)(axis & GamePadAxes.RightX) != 0)
            {
                _rightStickX = value;
            }
            if ((byte)(axis & GamePadAxes.RightY) != 0)
            {
                _rightStickY = value;
            }
            if ((byte)(axis & GamePadAxes.LeftTrigger) != 0)
            {
                _leftTrigger = (byte)(value - -32768 >> 8);
            }
            if ((byte)(axis & GamePadAxes.RightTrigger) != 0)
            {
                _rightTrigger = (byte)(value - -32768 >> 8);
            }
        }

        internal void SetButton(Buttons button, bool pressed)
        {
            if (pressed)
            {
                _buttons |= button;
                return;
            }
            _buttons &= ~button;
        }

        internal void SetConnected(bool connected)
        {
            _isConnected = connected;
        }

        internal void SetPacketNumber(int number)
        {
            _packetNumber = number;
        }

        internal void SetTriggers(byte left, byte right)
        {
            _leftTrigger = left;
            _rightTrigger = right;
        }

        public override string ToString()
        {
            return $"{{Sticks: {ThumbSticks}; Buttons: {Buttons}; DPad: {DPad}; IsConnected: {IsConnected}}}";
        }
    }
}

