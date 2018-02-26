using System;

namespace engenious.Helper
{
    public static class MathHelper
    {
        public const float Pi = (float)(Math.PI);
        public const float PiOver2 = (float)(Math.PI / 2);
        public const float PiOver4 = (float)(Math.PI / 4);
        public const float TwoPi = (float)(2 * Math.PI);

        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder(angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }
        public static float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        public static float Clamp(float val, float min, float max)
        {
            return val > max ? max : (val < min ? min : val);
        }
    }
}

