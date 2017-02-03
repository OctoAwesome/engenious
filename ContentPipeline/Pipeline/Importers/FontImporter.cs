using System;
using System.IO;
using System.Text;

namespace engenious.Content.Pipeline
{
    [ContentImporter(".fnt", DisplayName = "FontImporter", DefaultProcessor = "FontProcessor")]
    public class FontImporter : ContentImporter<FontContent>
    {
        public override FontContent Import(string filename, ContentImporterContext context)
        {
            try
            {
                string content = File.ReadAllText(filename, Encoding.UTF8);
                string toFind = "page id=0 file=\"";
                int start = content.IndexOf(toFind, StringComparison.Ordinal);
                if (start == -1)
                    throw new Exception("Not a valid font file");
                int end = content.IndexOf('\"', start + toFind.Length);
                if (end == -1)
                    throw new Exception("Not a valid font file");
                string texture = content.Substring(start + toFind.Length, end - (start + toFind.Length));
                start = content.IndexOf("common ", StringComparison.Ordinal);
                if (start == -1)
                    throw new Exception("Not a valid font file");

                content = content.Substring(start);

                return new FontContent(filename, Path.Combine(Path.GetDirectoryName(filename), texture), content);
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename,ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }
    }

    public class FontContent
    {
        public FontContent(string fileName, string textFile, string content)
        {
            FileName = fileName;
            TextureFile = textFile;
            Content = content;
        }

        public string FileName { get; set; }

        public string TextureFile { get; set; }

        public string Content { get; set; }
    }
}
