using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public interface ISaveCellExplorerContext
    {
        UniTask<ISceneDataExplorerContext> OpenSceneAsync(string sceneName);

        object GetData(Type type, bool createIfNot = false);
        T GetData<T>(bool createIfNot = false) where T : class;

        UniTask SaveAsync<T>(T data) where T : class;
        UniTask SaveAllAsync();
    }
}
