using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public interface IGlobalDataExplorerContext
    {
        UniTask<ISaveCellExplorerContext> OpenSaveCellAsync();
        UniTask<ISaveCellExplorerContext> OpenSaveCellAsync(int cellIndex);

        object GetData(Type type);
        T GetData<T>() where T : class;

        UniTask SaveAsync<T>(T data) where T : class;
        UniTask SaveAllAsync();
    }
}
