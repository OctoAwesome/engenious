using System.IO;
using engenious.Content;
using engenious.Content.Pipeline;
using engenious.Graphics;

namespace engenious.Pipeline
{
    [ContentImporter(".ego", DisplayName = "Model Importer", DefaultProcessor = "EgoModelProcessor")]
    public class EgoImporter : ContentImporter<ModelContent>
    {
        #region implemented abstract members of ContentImporter

        public override ModelContent Import(string filename, ContentImporterContext context)
        {

            ContentManager manager = new ContentManager(null,Path.GetDirectoryName(filename));
            return manager.Load<ModelContent>(Path.GetFileNameWithoutExtension(filename));
        }

        #endregion
    }
}

