using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace UnitySaveTool
{
    public interface ISaveCellExplorerContext
    {
        UniTask<ISceneDataExplorerContext> OpenScene(string sceneName);

        object GetData(Type type);
        T GetData<T>() where T : class;

        UniTask Save<T>(T data) where T : class;
    }
}
