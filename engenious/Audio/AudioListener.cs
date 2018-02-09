namespace engenious.Audio
{
    public class AudioListener
    {
        public AudioListener()
        {
            Position = new Vector3();
            Up = Vector3.UnitY;
            Forward = Vector3.UnitZ;
        }
            
        public Vector3 Forward { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Up { get; set; }

        public Vector3 Velocity { get; set; }
    }
}

