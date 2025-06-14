using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IDataExplorer
    {
        IGlobalDataContext GlobalDataSet { get; }
        ISaveCellContext SaveCellDataSet { get; }
        ISceneDataContext SceneDataSet { get; }

        void OpenSceneDataSet(string sceneName);
        void SaveAll();

        UniTask OpenSceneDataSetAsync(string sceneName);
        UniTask SaveAllAsync();
    }
}
