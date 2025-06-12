using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public class SaveToolBindInstaller : ISaveToolBindInstaller, IProvider
    {
        private readonly IDIContainer _globalContext;

        private readonly IDataExplorer _dataExplorer;

        public SaveToolBindInstaller(IDIContainer globalContext)
        {
            _globalContext = globalContext;

            InstallDefaultBindings();

            _dataExplorer = _globalContext.Resolve<IDataExplorer>();
        }

        private void InstallDefaultBindings()
        {
            if (_globalContext.HasBinding(typeof(IDataConverter)) == false)
                _globalContext.RegisterInstance<IDataConverter>(new JsonUtilityDataConverter());

            if (_globalContext.HasBinding(typeof(IFileSystem)) == false)
                _globalContext.RegisterInstance<IFileSystem>(new FileSystem(_globalContext.Resolve<IDataConverter>()));

            if (_globalContext.HasBinding(typeof(IDataExplorer)) == false)
                _globalContext.RegisterInstance<IDataExplorer>(new DataExplorer(_globalContext.Resolve<IFileSystem>()));
        }

        public async UniTask InstallDataProviderInSceneContext(string sceneName, IDIContainer sceneContext)
        {
            _dataExplorer.SceneDataSet?.SaveAll();

            await _dataExplorer.OpenSceneDataSet(sceneName);

            sceneContext.InstallProvdier(this);
        }

        public object GetInstance(Type typeToResolve)
        {
            return _dataExplorer.SceneDataSet.GetData(typeToResolve, true);
        }

        public bool HasInstance(Type typeToResolve)
        {
            if (_dataExplorer.SceneDataSet.GetAllDataTypes().Contains(typeToResolve))
                return true;

            if (typeToResolve.GetConstructor(Type.EmptyTypes) != null)
                return true;

            return false;
        }

        public Type GetInstanceType(Type typeToResolve)
        {
            if (HasInstance(typeToResolve))
                return typeToResolve;

            return null;
        }
    }
}