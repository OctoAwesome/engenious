using engenious.Content.Pipeline;

namespace engenious.Pipeline
{
    [ContentProcessor(DisplayName = "Passtrough Processor")]
    public class PassthroughProcessor : ContentProcessor<object,object>
    {
        #region implemented abstract members of ContentProcessor

        public override object Process(object input, string filename, ContentProcessorContext context)
        {
            return input;
        }

        #endregion
    }
}

