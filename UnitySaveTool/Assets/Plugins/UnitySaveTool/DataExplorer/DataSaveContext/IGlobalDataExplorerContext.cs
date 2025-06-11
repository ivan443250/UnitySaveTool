using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IGlobalDataExplorerContext
    {
        UniTask<ISaveCellExplorerContext> OpenSaveCell();
        UniTask<ISaveCellExplorerContext> OpenSaveCell(int cellIndex);

        object GetData(Type type);
        T GetData<T>() where T : class;

        UniTask Save<T>(T data) where T : class;
    }
}
