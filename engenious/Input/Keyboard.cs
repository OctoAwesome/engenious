using System;
using engenious.Helper;

namespace engenious.Input
{
    public static class Keyboard
    {
        static Keyboard()
        {
            if (!WrappingHelper.ValidateStructs<OpenTK.Input.KeyboardState,KeyboardState>())
                throw new TypeLoadException("Can't wrap OpenTK Keyboard two own internal struct");
        }
        public static unsafe KeyboardState GetState()
        {
            var state = OpenTK.Input.Keyboard.GetState();
            return *(KeyboardState*)(&state);


        }
        public static unsafe KeyboardState GetState(int index)
        {
            var state = OpenTK.Input.Keyboard.GetState(index);
            return *(KeyboardState*)(&state);


        }
    }
}

