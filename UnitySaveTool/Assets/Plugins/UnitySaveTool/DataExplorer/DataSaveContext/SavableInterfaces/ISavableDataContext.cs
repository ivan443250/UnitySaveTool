namespace UnitySaveTool
{
    public interface ISavableDataContext
    {
        void Save<T>(T data) where T : class;
        void SaveAll();
    }
}
