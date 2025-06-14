using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IAsyncSavableDataContext
    {
        UniTask SaveAsync<T>(T data) where T : class;
        UniTask SaveAllAsync();
    }
}