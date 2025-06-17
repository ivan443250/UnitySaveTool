using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public class DataExplorer : IDataExplorer
    {
        private readonly IGlobalDataContext _globalDataContext;

        public IGlobalDataContext GlobalDataSet => _globalDataContext;
        public ISaveCellContext SaveCellDataSet => _globalDataContext.OpenedSaveCellContext ?? GlobalDataSet.OpenSaveCell();
        public ISceneDataContext SceneDataSet => _globalDataContext.OpenedSaveCellContext?.OpenedSceneContext;

        public DataExplorer(IGlobalDataContext globalDataContext)
        {
            _globalDataContext = globalDataContext;
        }

        public void OpenSceneDataSet(string sceneName)
        {
            if (SaveCellDataSet == null)
                GlobalDataSet.OpenSaveCell();

            SaveCellDataSet.OpenScene(sceneName);
        }

        public void SaveAll()
        {
            if (SceneDataSet != null)
                SceneDataSet.SaveAll();

            if (SaveCellDataSet != null)
                SaveCellDataSet.SaveAll();

            GlobalDataSet.SaveAll();
        }

        async UniTask IDataExplorer.OpenSceneDataSetAsync(string sceneName)
        {
            if (SaveCellDataSet == null)
                await GlobalDataSet.OpenSaveCellAsync();

            await SaveCellDataSet.OpenSceneAsync(sceneName);
        }

        async UniTask IDataExplorer.SaveAllAsync()
        {
            if (SceneDataSet != null)
                await SceneDataSet.SaveAllAsync();

            if (SaveCellDataSet != null)
                await SaveCellDataSet.SaveAllAsync();

            await GlobalDataSet.SaveAllAsync();
        }
    }
}
