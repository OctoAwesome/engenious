using System;

namespace engenious.Pipeline.Pipeline.Editors
{
    public class ContentEditorWrapper//TODO: rename
    {
        public ContentEditorWrapper(IContentEditor editor,Action<object,object> open)
        {
            Editor = editor;
            Open = open;
        }
        public IContentEditor Editor { get; private set; }
        public Action<object, object> Open { get; private set; }
    }
}
