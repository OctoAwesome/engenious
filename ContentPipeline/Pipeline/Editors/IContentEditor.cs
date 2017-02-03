using System.Windows.Forms;

namespace engenious.Pipeline.Pipeline.Editors
{
    public interface IContentEditor<in TInput, in TOutput> : IContentEditor
    {
        void Open(TInput importerInput, TOutput processorOutput);
    }

    public interface IContentEditor
    {
        Control MainControl { get; }
    }
}
