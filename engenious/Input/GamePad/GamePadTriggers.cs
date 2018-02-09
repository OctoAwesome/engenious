using System;

namespace engenious.Input
{
    public struct GamePadTriggers : IEquatable<GamePadTriggers>
    {
        private const float ConversionFactor = 1.0f / byte.MaxValue;
        private readonly byte _left;

        private readonly byte _right;
        //
        // Properties
        //
        public float Left => _left * ConversionFactor;

        public float Right => _right * ConversionFactor;

        //
        // Constructors
        //
        internal GamePadTriggers(byte left, byte right)
        {
            _left = left;
            _right = right;
        }

        //
        // Methods
        //
        public override bool Equals(object obj)
        {
            return obj is GamePadTriggers && Equals((GamePadTriggers)obj);
        }

        public bool Equals(GamePadTriggers other)
        {
            return _left == other._left && _right == other._right;
        }

        public override int GetHashCode()
        {
            return _left.GetHashCode() ^ _right.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Left: {0:f2}; Right: {1:f2}}}", Left, Right);
        }

        //
        // Operators
        //
        public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
        {
            return !left.Equals(right);
        }
    }
}

