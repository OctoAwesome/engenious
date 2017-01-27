using System;
using OpenTK.Audio.OpenAL;

namespace engenious.Audio
{
    public class SoundEffectInstance : AudioResource
    {
        internal int sid;
        private bool isLooped;
        private float volume=1f,pitch=1f,pan;
        private SoundEffect effect;
        public SoundEffectInstance(SoundEffect effect)
        {
            this.effect = effect;
        }



        public TimeSpan Duration { get; }


        public bool IsLooped
        { 
            get { return isLooped; } 
            set
            { 
                if (value != isLooped && sid != 0)
                {
                    AL.Source(sid, ALSourceb.Looping, value);
                }
                isLooped = value;
            }
        }

        public float Volume{
            get{return volume;}
            set
            {
                if (volume != value)
                {
                    AL.Source (sid, ALSourcef.Gain, value);
                }
                volume = value;
            }
            
        }

        public float Pitch{
            get{return pitch;}
            set
            {
                if (pitch != value)
                {
                    AL.Source (sid, ALSourcef.Pitch,value);
                }
                pitch = value;
            }
        }

        public float Pan{
            get{return pan;}
            set
            {
                if (pan != value)
                {
                    AL.Source (sid, ALSource3f.Position,value, 0, 0.1f);
                }
                pan = value;
            }
        }

        public SoundState State { get; internal set; }

        public void Apply3D(AudioListener[] listeners, AudioEmitter emitter)
        {
            throw new NotSupportedException("Just one listener is supported!");
            //foreach(var l in listeners)
            //    Apply3D(l,emitter);
        }

        private void Apply3D(AudioListener listener,AudioEmitter emitter)
        {
            if (sid==0)
                return;


            float x, y, z;
            AL.GetListener(ALListener3f.Position, out x, out y, out z);

            Vector3 posOffset = emitter.Position - listener.Position;
            // set up orientation matrix
            Matrix orientation = Matrix.Identity;
            Vector3 forward = listener.Forward.Normalized();
            Vector3 right = Vector3.Cross(forward, listener.Up);
            Vector3 up = Vector3.Cross(right,forward);
            orientation.M11 = right.X;
            orientation.M12 = right.Y;
            orientation.M13 = right.Z;
            orientation.M21 = up.X;
            orientation.M22 = up.Y;
            orientation.M23 = up.Z;
            orientation.M31 = -forward.X;
            orientation.M32 = -forward.Y;
            orientation.M33 = -forward.Z;

            // set up our final position and velocity according to orientation of listener
            Vector3 finalPos = new Vector3(x + posOffset.X, y + posOffset.Y, z + posOffset.Z);
            finalPos = Vector3.Transform(finalPos, orientation);
            Vector3 finalVel = emitter.Velocity;
            finalVel = Vector3.Transform(finalVel, orientation);

            // set the position based on relative positon
            AL.Source(sid, ALSource3f.Position, finalPos.X, finalPos.Y, finalPos.Z);
            AL.Source(sid, ALSource3f.Velocity, finalVel.X, finalVel.Y, finalVel.Z);
        }


        public virtual void Play()
        {
            if (sid==0)
            {
                sid = SoundSourceManager.Instance.Dequeue();
                int bufferId = effect.Buffer;
                AL.Source(sid, ALSourcei.Buffer, bufferId);
            }

            if (sid==0)
                return;

            AL.DistanceModel (ALDistanceModel.InverseDistanceClamped);
            AL.Source (sid, ALSource3f.Position,Pan, 0, 0.1f);
            AL.Source (sid, ALSourcef.Gain, Volume);
            AL.Source (sid, ALSourceb.Looping, IsLooped);
            AL.Source (sid, ALSourcef.Pitch,Pitch);

            SoundSourceManager.Instance.PlaySound(this);
        }

        public void Pause()
        {
            State = SoundState.Paused;

            AL.SourcePause(sid);
        }

        public void Resume()
        {
            Play();
        }

        //private bool stopped = false;

        public void Stop(bool immediate = true)
        {
            //if (immediate)
            {
                AL.SourceStop(sid);
                AL.Source(sid, ALSourcei.Buffer, 0);
                SoundSourceManager.Instance.FreeSource(this);
            }
            //else
            //{
            //    stopped = true;//TODO: dunno?
            //}
        }

        #region IDisposable implementation

        public override void Dispose()
        {
            SoundSourceManager.Instance.FreeSource(this);
            base.Dispose();
        }

        #endregion


    }
}

