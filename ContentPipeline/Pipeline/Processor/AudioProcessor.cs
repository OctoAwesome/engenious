using System;
using System.ComponentModel;
using engenious.Content.Pipeline;
using System.IO;
using engenious.Audio;

namespace engenious.Pipeline
{
    [ContentProcessor(DisplayName = "Audio Processor")]
    public class AudioProcessor : ContentProcessor<FFmpegContent,AudioContent,AudioProcessorSettings>
    {
        public AudioProcessor()
        {
        }

        #region implemented abstract members of ContentProcessor

        public override AudioContent Process(FFmpegContent input, string filename, ContentProcessorContext context)
        {
            try
            {
                FFmpeg ff = new FFmpeg(context.SyncContext);
                string args = "";
                switch (settings.OutputFormat)
                {
                    case SoundEffect.AudioFormat.Ogg:
                        args = "-acodec libvorbis -ab 128k -ar 44100 -f ogg";
                        break;
                    case SoundEffect.AudioFormat.Wav:
                        args = "-acodec pcm_s16le -ar 44100 -f wav";
                        break;
                }
                var process = ff.RunCommand($"-i \"{filename}\" {args} -nostdin pipe:1 -hide_banner -loglevel error");
                var outputStream = process.StandardOutput.BaseStream as FileStream;

                var output = new AudioContent(settings.OutputFormat, outputStream);
                process.WaitForExit();
                var err = process.StandardError.ReadToEnd();//TODO: error handling
                if (!string.IsNullOrEmpty(err))
                {
                    context.RaiseBuildMessage(filename, "ffmpeg: " + err, BuildMessageEventArgs.BuildMessageType.Error);
                }
                return output;
            }catch (Win32Exception ex)
            {
                context.RaiseBuildMessage(filename, "ffmpeg: " + ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
                return null;
            }
        }

        #endregion
    }

    public class AudioProcessorSettings : ProcessorSettings
    {
        [Category("Settings")]
        [DefaultValue(SoundEffect.AudioFormat.Ogg)]
        public SoundEffect.AudioFormat OutputFormat { get; set; }
    }
}

