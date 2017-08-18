using System;
using OpenTK.Audio.OpenAL;

namespace engenious.Audio
{
    public class SoundEffectInstance : AudioResource
    {
        internal int Sid;
        protected bool _isLooped;
        protected float _volume=1f,_pitch=1f,_pan;
        protected SoundEffect _effect;

        public SoundEffectInstance()
            :this(null)
        {

        }
        public SoundEffectInstance(SoundEffect effect)
            : this(effect,TimeSpan.Zero)
        {

        }
        public SoundEffectInstance(SoundEffect effect, TimeSpan duration)
        {
            _effect = effect;
            Duration = duration;
        }



        public TimeSpan Duration { get; protected set; }


        public bool IsLooped
        { 
            get { return _isLooped; }
            set
            { 
                if (value != _isLooped && Sid != 0)
                {
                    AL.Source(Sid, ALSourceb.Looping, value);
                }
                _isLooped = value;
            }
        }

        public float Volume{
            get{return _volume;}
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_volume != value)
                {
                    AL.Source (Sid, ALSourcef.Gain, value);
                }
                _volume = value;
            }
            
        }

        public float Pitch{
            get{return _pitch;}
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pitch != value)
                {
                    AL.Source (Sid, ALSourcef.Pitch,value);
                }
                _pitch = value;
            }
        }

        public float Pan{
            get{return _pan;}
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pan != value)
                {
                    AL.Source (Sid, ALSource3f.Position,value, 0, 0.1f);
                }
                _pan = value;
            }
        }

        public virtual SoundState State { get; internal set; }

        public void Apply3D(AudioListener[] listeners, AudioEmitter emitter)
        {
            throw new NotSupportedException("Just one listener is supported!");
            //foreach(var l in listeners)
            //    Apply3D(l,emitter);
        }

        private void Apply3D(AudioListener listener,AudioEmitter emitter)
        {
            if (Sid==0)
                return;


            float x, y, z;
            AL.GetListener(ALListener3f.Position, out x, out y, out z);

            var posOffset = emitter.Position - listener.Position;
            // set up orientation matrix
            var orientation = Matrix.Identity;
            var forward = listener.Forward.Normalized();
            var right = Vector3.Cross(forward, listener.Up);
            var up = Vector3.Cross(right,forward);
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
            var finalPos = new Vector3(x + posOffset.X, y + posOffset.Y, z + posOffset.Z);
            finalPos = Vector3.Transform(finalPos, orientation);
            var finalVel = emitter.Velocity;
            finalVel = Vector3.Transform(finalVel, orientation);

            // set the position based on relative positon
            AL.Source(Sid, ALSource3f.Position, finalPos.X, finalPos.Y, finalPos.Z);
            AL.Source(Sid, ALSource3f.Velocity, finalVel.X, finalVel.Y, finalVel.Z);
        }


        public virtual void Play()
        {
            if (Sid==0)
            {
                Sid = SoundSourceManager.Instance.Dequeue();
                var bufferId = _effect.Buffer;
                AL.Source(Sid, ALSourcei.Buffer, bufferId);
            }

            if (Sid==0)
                return;

            AL.DistanceModel (ALDistanceModel.InverseDistanceClamped);
            AL.Source (Sid, ALSource3f.Position,Pan, 0, 0.1f);
            AL.Source (Sid, ALSourcef.Gain, Volume);
            AL.Source (Sid, ALSourceb.Looping, IsLooped);
            AL.Source (Sid, ALSourcef.Pitch,Pitch);

            SoundSourceManager.Instance.PlaySound(this);
        }

        public void Pause()
        {
            State = SoundState.Paused;

            AL.SourcePause(Sid);
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
                AL.SourceStop(Sid);
                AL.Source(Sid, ALSourcei.Buffer, 0);
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

