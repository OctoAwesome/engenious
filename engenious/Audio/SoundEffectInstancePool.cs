using System;
using System.Collections.Generic;

namespace engenious.Audio
{
    public class SoundEffectInstancePool
    {
        private static SoundEffectInstancePool _instance;
        public static SoundEffectInstancePool Instance => _instance ?? (_instance = new SoundEffectInstancePool());

        private const int InstanceCount = 256;
        private readonly Stack<PooledSoundEffectInstance> _stack = new Stack<PooledSoundEffectInstance>(InstanceCount);

        public SoundEffectInstancePool()
        {
            for (var i = 0; i < InstanceCount; i++)
            {
                _stack.Push(new PooledSoundEffectInstance());
            }
        }
        public SoundEffectInstance Aquire(SoundEffect effect)
        {
            var inst = _stack.Count == 0 ? new PooledSoundEffectInstance() : _stack.Pop();
            inst.Init(effect);
            return inst;
        }

        public void Release(PooledSoundEffectInstance instance)
        {
            _stack.Push(instance);
        }
    }

    public class PooledSoundEffectInstance : SoundEffectInstance
    {
        internal PooledSoundEffectInstance()
            :base()
        {

        }

        internal void Init(SoundEffect effect)
        {
            _effect = effect;
            Duration = TimeSpan.Zero;
            _isLooped = false;
            _volume = 1f;
            _pitch = 1f;
            _pan = 0;
        }

        private SoundState _state;

        public override SoundState State
        {
            get { return _state; }
            internal set
            {
                _state = value;
                if (value == SoundState.Stopped)
                    SoundEffectInstancePool.Instance.Release(this);
            }
        }
    }
}