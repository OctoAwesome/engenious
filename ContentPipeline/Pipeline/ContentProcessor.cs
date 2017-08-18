using System;

namespace engenious.Content.Pipeline
{
    public abstract class ContentProcessor<TInput,TOutput,TSettings> : IContentProcessor where TSettings:ProcessorSettings,new()
    {
        protected ContentProcessor()
        {
            settings = new TSettings();
        }
        protected static readonly Type _importType =  typeof(TInput);
        protected static readonly Type _exportType = typeof(TOutput);
        public Type ImportType => _importType;
        public Type ExportType => _exportType;

        /*public TSettings Settings
        {
            get;set;
        }*/
        [System.Xml.Serialization.XmlIgnore()]
        protected TSettings settings;

        public ProcessorSettings Settings{
            get
            {
                return settings;
            }
            set
            {
                settings = (TSettings)value;
            }
        }

        public abstract TOutput Process(TInput input,string filename, ContentProcessorContext context);

        object IContentProcessor.Process(object input, string filename, ContentProcessorContext context)
        {
            return Process((TInput)input,filename, context);
        }
    }
    public abstract class ContentProcessor<TInput,TOutput> : ContentProcessor<TInput,TOutput,ProcessorSettings>
    {
        public ContentProcessor()
        {
            Settings = null;
        }
    }
}

