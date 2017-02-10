using System;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace engenious.Audio
{
    public class SoundSourceManager
    {

        private static SoundSourceManager _instance;
        public static SoundSourceManager Instance => _instance ?? (_instance = new SoundSourceManager());


        private const int MaxSources=256;
        private readonly Thread _updateThread;
        private readonly int[] _sources;
        private readonly List<SoundEffectInstance> _playingInstances;
        private readonly List<int> _inUse;
        private readonly List<int> _available;
        private readonly List<int> _playing;
        public SoundSourceManager()
        {
            _sources = AL.GenSources(MaxSources);

            _inUse = new List<int>();
            _playing = new List<int>();
            _available = new List<int>(_sources);
            _playingInstances = new List<SoundEffectInstance>();
            _updateThread = new Thread(UpdateLoop){IsBackground = true};
            _updateThread.Start();
        }

        private void UpdateLoop()
        {
            while (true)
            {
                Update();
                Thread.Sleep(100);
            }
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
                    var sei = _playingInstances[i];
                    if (AL.GetSourceState(sid) == ALSourceState.Stopped)
                    {
                        _playing.RemoveAt(i);
                        AL.Source(sid, ALSourcei.Buffer, 0);

                        Enqueue(sid);
                        sei.Sid = 0;
                        sei.State = SoundState.Stopped;
                        _playingInstances.RemoveAt(i);
                    }
                }
            }
        }

        public void PlaySound(SoundEffectInstance inst)
        {
            lock (_playing)
            {
                _playing.Add(inst.Sid);
                _playingInstances.Add(inst);
            }
            AL.SourcePlay(inst.Sid);
            inst.State = SoundState.Playing;

        }
        public void FreeSource(SoundEffectInstance inst)
        {
            lock (_playing) {
                _playing.Remove(inst.Sid);
            }
            AL.Source(inst.Sid, ALSourcei.Buffer, 0);
            Enqueue(inst.Sid);
            inst.Sid = 0;
            inst.State = SoundState.Stopped;
        }
    }
}

