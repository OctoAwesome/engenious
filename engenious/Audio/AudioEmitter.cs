namespace engenious.Audio
{
    public class AudioEmitter
    {
        public AudioEmitter()
        {
            Position = new Vector3();
            Up = Vector3.UnitY;
            Forward = Vector3.UnitZ;

            DopplerScale = 1.0f;
        }

        public float DopplerScale{ get; set; }

        public Vector3 Forward { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Up { get; set; }

        public Vector3 Velocity { get; set; }
    }
}

