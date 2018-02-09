using System;
using NVorbis;

namespace engenious.Audio
{
    internal class OggStream : IDisposable
    {
        internal VorbisReader Reader { get; }
        public OggStream(System.IO.Stream stream,bool closeStreamOnDispose=false)
        {
            Reader = new VorbisReader(stream,closeStreamOnDispose);
        }
        public OggStream(string filename)
        {
            Reader = new VorbisReader(filename);
        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}

