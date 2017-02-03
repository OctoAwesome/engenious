using OpenTK.Audio;

namespace engenious.Audio
{
    public class AudioDevice : AudioResource
    {
        internal AudioContext Context;

        public AudioDevice()
        {
            Context = new AudioContext();
            Context.MakeCurrent();
        }

        public override void Dispose()
        {
            Context.Dispose();
            base.Dispose();
        }
    }
}

