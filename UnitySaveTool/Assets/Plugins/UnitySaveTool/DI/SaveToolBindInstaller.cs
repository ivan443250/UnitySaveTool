using Cysharp.Threading.Tasks;
using System;
using System.Reflection;
using UnityEngine;

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
            Debug.Log("1");

            if (_dataExplorer.SceneDataSet != null)
                await _dataExplorer.SceneDataSet.SaveAllAsync();

            await _dataExplorer.OpenSceneDataSetAsync(sceneName);

            sceneContext.RegisterProvdier(this);

            Debug.Log("2");
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

            if (typeToResolve.GetConstructor(Type.EmptyTypes) != null && typeToResolve.GetCustomAttribute<SaveToolDataAttribute>() != null)
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