using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnitySaveTool
{
    public class DefaultDataExplorer : IDataExplorer, 
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

        public DefaultDataExplorer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _saveCellIndex = -1;
        }

        #region IGlobalDataExplorerContext API

        async Task<ISaveCellExplorerContext> IGlobalDataExplorerContext.OpenSaveCell()
        {
            _globalContext = await _fileSystem.LoadAll();

            if (_globalContext.ContainsKey(typeof(SavedContext)) == false)
                return await OpenSaveCellInternal(0);
            else
                return await OpenSaveCellInternal((_globalContext[typeof(SavedContext)] as SavedContext).SaveCell);
        }

        async Task<ISaveCellExplorerContext> IGlobalDataExplorerContext.OpenSaveCell(int cellIndex)
        {
            return await OpenSaveCellInternal(cellIndex);
        }

        object IGlobalDataExplorerContext.GetData(Type type)
        {
            return GetDataInternal(type, _globalContext);
        }

        async Task IGlobalDataExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _globalContext);
        }

        private async Task<ISaveCellExplorerContext> OpenSaveCellInternal(int cellIndex)
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

        async Task<ISceneDataExplorerContext> ISaveCellExplorerContext.OpenScene(string sceneName)
        {
            _sceneName = sceneName;

            _sceneContext = await _fileSystem.LoadAll(_saveCellIndex.ToString(), sceneName);

            return this;
        }

        object ISaveCellExplorerContext.GetData(Type type)
        {
            return GetDataInternal(type, _cellContext);
        }

        async Task ISaveCellExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _cellContext, _saveCellIndex.ToString());
        }

        #endregion

        #region ISceneDataExplorerContext API

        object ISceneDataExplorerContext.GetData(Type type)
        {
            return GetDataInternal(type, _sceneContext);
        }

        async Task ISceneDataExplorerContext.Save<T>(T data) where T : class
        {
            await SaveDataInternal(data, _sceneContext, _saveCellIndex.ToString(), _sceneName);
        }

        #endregion

        private object GetDataInternal(Type type, Dictionary<Type, object> context)
        {
            if (context.ContainsKey(type) == false)
                return null;

            return context[type];
        }

        private async Task SaveDataInternal(object data, Dictionary<Type, object> context, params string[] foldersPath)
        {
            Type dataType = data.GetType();

            if (context.ContainsKey(dataType) == false)
                context.Add(dataType, null);

            context[dataType] = data;

            await _fileSystem.Save(data, foldersPath);
        }

        public async Task OpenSceneDataSet(string sceneName)
        {
            if (SaveCellDataSet == null)
                await GlobalDataSet.OpenSaveCell();

            await SaveCellDataSet.OpenScene(sceneName);
        }

        public async Task SaveAll()
        {
            if (SceneDataSet != null)
                await _fileSystem.SaveAll(_sceneContext, _saveCellIndex.ToString(), _sceneName);

            if (SaveCellDataSet != null)
                await _fileSystem.SaveAll(_cellContext, _saveCellIndex.ToString());

            await _fileSystem.SaveAll(_globalContext);
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

    public interface IGlobalDataExplorerContext
    {
        Task<ISaveCellExplorerContext> OpenSaveCell();
        Task<ISaveCellExplorerContext> OpenSaveCell(int cellIndex);

        object GetData(Type type);
        Task Save<T>(T data) where T : class;
    }

    public interface ISaveCellExplorerContext
    {
        Task<ISceneDataExplorerContext> OpenScene(string sceneName);

        object GetData(Type type);
        Task Save<T>(T data) where T : class;
    }

    public interface ISceneDataExplorerContext
    {
        object GetData(Type type);
        Task Save<T>(T data) where T : class;
    }
}
