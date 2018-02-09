namespace engenious.Input
{
    public class GamePad
    {
        public static unsafe GamePadState GetState(int index = 0)
        {
            var state = OpenTK.Input.GamePad.GetState(index);

            var actual = *(GamePadState*)&state;
            //TODO:actual.ThumbSticks.Left = new Vector2(Math.M
            return actual;
        }
    }
}

