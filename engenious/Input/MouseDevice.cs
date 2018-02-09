namespace engenious
{
    public class MouseDevice
    {
        private readonly OpenTK.Input.MouseDevice _dev;

        public MouseDevice(OpenTK.Input.MouseDevice dev)
        {
            _dev = dev;
        }

        public int X => _dev.X;

        public int Y => _dev.Y;
    }
}

