using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySaveTool
{
    public class DataExplorer : IDataExplorer,
        IGlobalDataExplorerContext, ISaveCellExplorerContext, ISceneDataExplorerContext
    {
        private readonly IFileSystem _fileSystem;

        private string _sceneName;

        private int _saveCellIndex;

        private Dictionary<Type, object> _globalContext;
        private Dictionary<Type, object> _cellContext;
        private Dictionary<Type, object> _sceneContext;

        #region APIs To Get Context

        public IGlobalDataExplorerContext GlobalDataSet => this;

        public ISaveCellExplorerContext SaveCellDataSet
        {
            get
            {
                if (_saveCellIndex == -1)
                    return null;

                return this;
            }
        }

        public ISceneDataExplorerContext SceneDataSet
        {
            get
            {
                if (SaveCellDataSet == null)
                    return null;

                if (_sceneName == null)
                    return null;

                return this;
            }
        }

        #endregion

        public DataExplorer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _saveCellIndex = -1;
        }

        #region IGlobalDataExplorerContext API

        async UniTask<ISaveCellExplorerContext> IGlobalDataExplorerContext.OpenSaveCell()
        {
            _globalContext = await _fileSystem.LoadAll();

            if (_globalContext.ContainsKey(typeof(SavedContext)) == false)
                return await OpenSaveCellInternal(0);
            else
                return await OpenSaveCellInternal((_globalContext[typeof(SavedContext)] as SavedContext).SaveCell);
        }

        async UniTask<ISaveCellExplorerContext> IGlobalDataExplorerContext.OpenSaveCell(int cellIndex)
        {
            return await OpenSaveCellInternal(cellIndex);
        }

        object IGlobalDataExplorerContext.GetData(Type type)
        {
            return GetDataInternal(type, _globalContext);
        }

        T IGlobalDataExplorerContext.GetData<T>()
        {
            return GetDataInternal(typeof(T), _globalContext) as T;
        }

        async UniTask IGlobalDataExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _globalContext);
        }

        async UniTask IGlobalDataExplorerContext.SaveAll()
        {
            await _fileSystem.SaveAll(_globalContext);
        }

        private async UniTask<ISaveCellExplorerContext> OpenSaveCellInternal(int cellIndex)
        {
            if (cellIndex < 0)
                throw new ArgumentOutOfRangeException("Save Cell Index can not be less than 0");

            await _fileSystem.Save(new SavedContext(cellIndex));

            _globalContext = await _fileSystem.LoadAll();

            _saveCellIndex = cellIndex;

            _cellContext = await _fileSystem.LoadAll(cellIndex.ToString());

            return this;
        }

        #endregion

        #region ISaveCellExplorerContext API    

        async UniTask<ISceneDataExplorerContext> ISaveCellExplorerContext.OpenScene(string sceneName)
        {
            _sceneName = sceneName;

            _sceneContext = await _fileSystem.LoadAll(_saveCellIndex.ToString(), sceneName);

            return this;
        }

        object ISaveCellExplorerContext.GetData(Type type, bool createIfNot)
        {
            return GetDataInternal(type, _cellContext, createIfNot);
        }

        T ISaveCellExplorerContext.GetData<T>(bool createIfNot)
        {
            return GetDataInternal(typeof(T), _cellContext, createIfNot) as T;
        }

        async UniTask ISaveCellExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _cellContext, _saveCellIndex.ToString());
        }

        async UniTask ISaveCellExplorerContext.SaveAll()
        {
            await _fileSystem.SaveAll(_cellContext, _saveCellIndex.ToString());
        }

        #endregion

        #region ISceneDataExplorerContext API

        object ISceneDataExplorerContext.GetData(Type type, bool createIfNot)
        {
            return GetDataInternal(type, _sceneContext, createIfNot);
        }

        T ISceneDataExplorerContext.GetData<T>(bool createIfNot)
        {
            return GetDataInternal(typeof(T), _sceneContext, createIfNot) as T;
        }

        HashSet<Type> ISceneDataExplorerContext.GetAllDataTypes()
        {
            return new HashSet<Type>(_sceneContext.Keys);
        }

        async UniTask ISceneDataExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _sceneContext, _saveCellIndex.ToString(), _sceneName);
        }

        async UniTask ISceneDataExplorerContext.SaveAll()
        {
            await _fileSystem.SaveAll(_sceneContext, _saveCellIndex.ToString(), _sceneName);
        }

        #endregion

        private object GetDataInternal(Type type, Dictionary<Type, object> context, bool createIfNot = false)
        {
            if (context.ContainsKey(type) == false && (type.GetConstructor(Type.EmptyTypes) == null || createIfNot == false))
                return null;

            if (context.ContainsKey(type) == false)
                context.Add(type, Activator.CreateInstance(type));

            return context[type];
        }

        private async UniTask SaveDataInternal(object data, Dictionary<Type, object> context, params string[] foldersPath)
        {
            Type dataType = data.GetType();

            if (context.ContainsKey(dataType) == false)
                context.Add(dataType, null);

            context[dataType] = data;

            await _fileSystem.Save(data, foldersPath);
        }

        async UniTask IDataExplorer.OpenSceneDataSet(string sceneName)
        {
            if (SaveCellDataSet == null)
                await GlobalDataSet.OpenSaveCell();

            await SaveCellDataSet.OpenScene(sceneName);
        }

        async UniTask IDataExplorer.SaveAll()
        {
            if (SceneDataSet != null)
                await SceneDataSet.SaveAll();

            if (SaveCellDataSet != null)
                await SaveCellDataSet.SaveAll();

            await GlobalDataSet.SaveAll();
        }
    }

    [Serializable]
    public class SavedContext
    {
        public int SaveCell => _saveCell;
        [SerializeField] private int _saveCell;

        public SavedContext() { }

        public SavedContext(int saveCell)
        {
            _saveCell = saveCell;
        }
    }
}
