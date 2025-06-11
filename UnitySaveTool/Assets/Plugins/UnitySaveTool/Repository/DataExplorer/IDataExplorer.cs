using System.Threading.Tasks;
using UnityEngine;

namespace UnitySaveTool
{
    public interface IDataExplorer 
    {
        IGlobalDataExplorerContext GlobalDataSet { get; }
        ISaveCellExplorerContext SaveCellDataSet { get; }
        ISceneDataExplorerContext SceneDataSet { get; }

        Task OpenSceneDataSet(string sceneName);
        Task SaveAll();
    }
}
