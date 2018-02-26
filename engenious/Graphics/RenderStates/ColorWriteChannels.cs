using System;

namespace engenious.Graphics
{
    [Flags]
    public enum ColorWriteChannels
    {
        None,
        Alpha=1,
        Blue=2,
        Green=4,
        Red=8,
        All=Alpha|Blue|Green|Red
    }
}