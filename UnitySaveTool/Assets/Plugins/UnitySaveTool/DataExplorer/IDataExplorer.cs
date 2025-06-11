using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IDataExplorer
    {
        IGlobalDataExplorerContext GlobalDataSet { get; }
        ISaveCellExplorerContext SaveCellDataSet { get; }
        ISceneDataExplorerContext SceneDataSet { get; }

        UniTask OpenSceneDataSet(string sceneName);
        UniTask SaveAll();
    }
}
