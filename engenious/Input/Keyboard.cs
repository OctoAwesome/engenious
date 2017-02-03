using System;

namespace engenious.Input
{
    public static class Keyboard
    {
        static Keyboard()
        {
            if (!WrappingHelper.ValidateStructs<OpenTK.Input.KeyboardState,KeyboardState>())
                throw new Exception("test");
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

