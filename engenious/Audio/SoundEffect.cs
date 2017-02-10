using System;
using System.IO;
using System.Runtime.CompilerServices;
using OpenTK.Audio.OpenAL;

namespace engenious.Audio
{
    public class SoundEffect : IDisposable
    {
        public static float SpeedOfSound { get; set; }

        public static float MasterVolume { get; set; }

        static SoundEffect()
        {
            SpeedOfSound = 345.5f;
            MasterVolume = 1.0f;
        }
        //TODO dynamic sound
        internal int Buffer;
        public SoundEffect(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                LoadStream(fs);
            }
        }

        public enum AudioFormat
        {
            Ogg,
            Wav
        }

        public SoundEffect(Stream stream, AudioFormat format)
        {
            LoadStream(stream,format);
        }
        public SoundEffect(Stream stream)
        {
            LoadStream(stream);
        }

        private void LoadStream(Stream stream)
        {
            LoadStream(stream,ReadAudioFormat(stream));
        }

        private void LoadStream(Stream stream, AudioFormat format)
        {
            //TODO: dynamic sound - buffer encapsulated
            AL.GenBuffers(1, out Buffer);
            switch (format)
            {
                case AudioFormat.Wav:
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        ALFormat alFormat;
                        int size, frequency;
                        var buffer = LoadWave(reader, out alFormat, out size, out frequency);
                        AL.BufferData(Buffer, alFormat, buffer, size, frequency);
                    }
                    break;
                case AudioFormat.Ogg:
                    using (OggStream ogg = new OggStream(stream))
                    {
                        int bitsPerSample = 16;
                        ALFormat alFormat = GetSoundFormat(ogg.Reader.Channels, bitsPerSample);
                        int frequency = ogg.Reader.SampleRate;
                        int samples = (int)(((long)ogg.Reader.TotalTime.TotalMilliseconds * ogg.Reader.SampleRate * ogg.Reader.Channels)/1000);

                        var conversionBuffer = new float[samples];
                        var buffer = new byte[samples*bitsPerSample/8];

                        ogg.Reader.ReadSamples(conversionBuffer, 0, samples);
                        unsafe
                        {
                            fixed (byte* ptr = buffer)
                            {
                                byte* ptr2 = ptr;
                                for (int i = 0; i < samples; i++,ptr2+=(bitsPerSample/8))
                                {
                                    ConvertShit(conversionBuffer[i],ptr2);
                                }
                            }
                        }
                        AL.BufferData(Buffer,alFormat,buffer,(buffer.Length - buffer.Length % 4),frequency);
                    }
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void ConvertShit(float val,byte* ptr)
        {
            val += 1;
            val = (val * ushort.MaxValue / 2) + short.MinValue;

            short tmp = (short) val;
            *ptr++ = (byte)(tmp & 0xFF);
            *ptr = (byte) (tmp >> 8);
        }
        private static AudioFormat ReadAudioFormat(Stream stream)
        {
            if (stream.CanSeek)
            {
                byte[] magic = new byte[4];
                stream.Read(magic, 0, magic.Length);

                AudioFormat format;
                if (magic[0] == 'R' && magic[1] == 'I' && magic[2] == 'F' && magic[3] == 'F')
                {
                    format = AudioFormat.Wav;
                }
                else if (magic[0] == 'O' && magic[1] == 'g' && magic[2] == 'g' && magic[3] == 'S')
                {
                    format = AudioFormat.Ogg;
                }
                else
                {
                    throw new NotSupportedException("Fileformat not supported!");
                }
                stream.Seek(-4, SeekOrigin.Current);
                return format;
            }
            throw new ArgumentException("Can't determine audio format of unseekable stream");
        }
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        private static byte[] LoadWave(BinaryReader reader, out ALFormat format, out int size, out int frequency)
        {
            // code based on opentk exemple

            byte[] audioData;

            //header
            string signature = new string(reader.ReadChars(4));
            if (signature != "RIFF")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            reader.ReadInt32(); // riff_chunck_size

            string wformat = new string(reader.ReadChars(4));
            if (wformat != "WAVE")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            // WAVE header
            string formatSignature = new string(reader.ReadChars(4));
            while (formatSignature != "fmt ")
            {
                reader.ReadBytes(reader.ReadInt32());
                formatSignature = new string(reader.ReadChars(4));
            }

            int formatChunkSize = reader.ReadInt32();

            // total bytes read: tbp
            int audioFormat = reader.ReadInt16(); // 2
            int numChannels = reader.ReadInt16(); // 4
            int sampleRate = reader.ReadInt32(); // 8
            reader.ReadInt32(); // 12, byte_rate
            reader.ReadInt16(); // 14, block_align
            int bitsPerSample = reader.ReadInt16(); // 16

            if (audioFormat != 1)
            {
                throw new NotSupportedException("Wave compression is not supported.");
            }

            // reads residual bytes
            if (formatChunkSize > 16)
                reader.ReadBytes(formatChunkSize - 16);

            string dataSignature = new string(reader.ReadChars(4));

            while (dataSignature.ToLowerInvariant() != "data")
            {
                reader.ReadBytes(reader.ReadInt32());
                dataSignature = new string(reader.ReadChars(4));
            }

            if (dataSignature != "data")
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }

            int dataChunkSize = reader.ReadInt32();

            frequency = sampleRate;
            format = GetSoundFormat(numChannels, bitsPerSample);
            audioData = reader.ReadBytes((int) reader.BaseStream.Length);
            if (dataChunkSize == -1)
                size = audioData.Length;
            else
                size = dataChunkSize;

            return audioData;
        }

        /*public System.IO.Stream OpenStream()
        {
            return null;
        }*/
        public void Play()
        {
            SoundEffectInstancePool.Instance.Aquire(this).Play();
        }
        public virtual SoundEffectInstance CreateInstance()
        {
            return SoundEffectInstancePool.Instance.Aquire(this);
        }
        #region IDisposable implementation
        public void Dispose()
        {
            AL.DeleteBuffer(Buffer);
        }
        #endregion
    }
}

