using engenious.Content.Pipeline;

namespace engenious.Pipeline
{
    [ContentImporter(".wav",".ogg",".mp3", DisplayName = "Wave Importer", DefaultProcessor = "AudioProcessor")]
    public class WaveImporter : ContentImporter<FFmpegContent>
    {
        #region implemented abstract members of ContentImporter

        public override FFmpegContent Import(string filename, ContentImporterContext context)
        {
            return new FFmpegContent(filename);
        }

        #endregion
    }
}

