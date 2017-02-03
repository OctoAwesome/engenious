using System;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;
using System.Linq;

namespace engenious.Audio
{
    public class SoundSourceManager
    {
        private static SoundSourceManager _instance;
        public static SoundSourceManager Instance => _instance ?? (_instance = new SoundSourceManager());


        private const int MaxSources=256;

        private readonly int[] _sources;
        private readonly List<int> _inUse;
        private readonly List<int> _available;
        private readonly List<int> _playing;
        public SoundSourceManager()
        {
            _sources = AL.GenSources(MaxSources);

            _inUse = new List<int>();
            _playing = new List<int>();
            _available = new List<int>(_sources);
        }

        public int Dequeue()
        {
            int source = _available.Last();
            _available.RemoveAt(_available.Count-1);
            _inUse.Add(source);
            return source;
        }
        public void Enqueue(int source)
        {
            _inUse.Remove(source);
            _available.Add(source);
        }
        public void Enqueue(int[] sources)
        {
            foreach(var source in sources)
            {
                Enqueue(source);
            }
        }

        public void Update()
        {
            lock(_playing){
                for (int i=_playing.Count-1;i>=0;i--)
                {
                    int sid = _playing[i];
                    if (AL.GetSourceState(sid) == ALSourceState.Stopped)
                    {
                        _playing.RemoveAt(sid);
                        AL.Source(sid, ALSourcei.Buffer, 0);

                        _inUse.Remove(sid);
                        _available.Add(sid);
                    }
                }
            }
        }

        public void PlaySound(SoundEffectInstance inst)
        {
            lock (_playing)
            {
                _playing.Add(inst.Sid);
            }
            AL.SourcePlay(inst.Sid);
            inst.State = SoundState.Playing;
        }
        public void FreeSource(SoundEffectInstance inst)
        {
            lock (_playing) {
                _playing.Remove(inst.Sid);
            }
            Enqueue(inst.Sid);
            inst.Sid = 0;
            inst.State = SoundState.Stopped;
        }
    }
}

