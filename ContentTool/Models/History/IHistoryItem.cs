namespace ContentTool.Models.History
{
    public interface IHistoryItem
    {
        void Undo();
        void Redo();
    }
}