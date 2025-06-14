using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public class SaveToolBindInstaller : ISaveToolBindInstaller, IProvider
    {
        private readonly IDIContainer _globalContext;

        private readonly IDataExplorer _dataExplorer;

        public SaveToolBindInstaller(IDIContainer globalContext, IDataExplorer dataExplorer)
        {
            _globalContext = globalContext;

            _dataExplorer = dataExplorer;
        }

        public async UniTask InstallDataProviderInSceneContextAsync(string sceneName, IDIContainer sceneContext)
        {
            await _dataExplorer.OpenSceneDataSetAsync(sceneName);

            sceneContext.RegisterProvdier(this);
        }

        public void InstallDataProviderInSceneContext(string sceneName, IDIContainer sceneContext)
        {
            _dataExplorer.OpenSceneDataSet(sceneName);

            sceneContext.RegisterProvdier(this);
        }

        public object GetInstance(Type typeToResolve)
        {
            if (HasInstance(typeToResolve))
                return _dataExplorer.SceneDataSet.GetData(typeToResolve, true);

            return null;
        }

        public bool HasInstance(Type typeToResolve)
        {
            if (_dataExplorer.SceneDataSet.GetAllDataTypes().Contains(typeToResolve))
                return true;

            return _dataExplorer.SceneDataSet.CheckTypeAddCondition(typeToResolve);
        }

        public Type GetInstanceType(Type typeToResolve)
        {
            if (HasInstance(typeToResolve))
                return typeToResolve;

            return null;
        }
    }
}