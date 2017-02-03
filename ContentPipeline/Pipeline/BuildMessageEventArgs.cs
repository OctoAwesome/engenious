namespace engenious.Content.Pipeline
{
    public class BuildMessageEventArgs
    {
        public enum BuildMessageType
        {
            None,
            Warning,
            Error,
            Information
        }
        public BuildMessageEventArgs(string filename,string message,BuildMessageType messageType)
        {
            FileName = filename;
            Message = message;
            MessageType = messageType;
        }
        public string FileName { get; private set; }
        public string Message { get; private set; }
        public BuildMessageType MessageType { get; private set; }
    }
}
