using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace UnitySaveTool
{
    public interface ISceneDataExplorerContext
    {
        object GetData(Type type);
        T GetData<T>() where T : class;

        UniTask Save<T>(T data) where T : class;
    }
}
