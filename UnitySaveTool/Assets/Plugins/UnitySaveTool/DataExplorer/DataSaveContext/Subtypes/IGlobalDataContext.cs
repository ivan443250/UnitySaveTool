using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IGlobalDataContext : ISceneDataContext
    {
        ISaveCellContext OpenSaveCell();
        ISaveCellContext OpenSaveCell(int cellIndex);

        UniTask<ISaveCellContext> OpenSaveCellAsync();
        UniTask<ISaveCellContext> OpenSaveCellAsync(int cellIndex);

        ISaveCellContext OpenedSaveCellContext { get; }
    }
}
