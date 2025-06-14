using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnitySaveTool
{
    public class GlobalDataContext : DataContext, IGlobalDataContext
    {
        public ISaveCellContext OpenedSaveCellContext => _cachedSaveCell;

        protected override string ContextName => nameof(GlobalDataContext);

        protected override SaveContext ContextType => SaveContext.Global;

        private SaveCellDataContext _cachedSaveCell;
        private int _cachedSaveCellIndex = -1;

        public GlobalDataContext(IFileSystem fileSystem,
            IAsyncFileSystem asyncFileSystem)
            : base(fileSystem.LoadAll(), fileSystem, asyncFileSystem) { }

        public ISaveCellContext OpenSaveCell()
        {
            if (GetAllDataTypes().Contains(typeof(SavedContext)))
                return OpenSaveCellInternal(GetData<SavedContext>().SaveCell);
            else
                return OpenSaveCellInternal(0);
        }

        public ISaveCellContext OpenSaveCell(int cellIndex)
        {
            return OpenSaveCellInternal(cellIndex);
        }

        public async UniTask<ISaveCellContext> OpenSaveCellAsync()
        {
            if (GetAllDataTypes().Contains(typeof(SavedContext)))
                return await OpenSaveCellInternalAsync(GetData<SavedContext>().SaveCell);
            else
                return await OpenSaveCellInternalAsync(0);
        }

        public async UniTask<ISaveCellContext> OpenSaveCellAsync(int cellIndex)
        {
            return await OpenSaveCellInternalAsync(cellIndex);
        }

        private ISaveCellContext OpenSaveCellInternal(int cellIndex)
        {
            if (CheckCache(cellIndex, out ISaveCellContext instantResult))
                return instantResult;

            if (_cachedSaveCell != null)
                _cachedSaveCell.SaveAll();

            SavedContext savedContext = new SavedContext(cellIndex);

            FileSystem.Save(savedContext);

            Dictionary<Type, object> cellContext = FileSystem.LoadAll(cellIndex.ToString());

            _cachedSaveCell = new SaveCellDataContext(cellContext, FileSystem, AsyncFileSystem, cellIndex);
            _cachedSaveCellIndex = cellIndex;

            return _cachedSaveCell;
        }

        private async UniTask<ISaveCellContext> OpenSaveCellInternalAsync(int cellIndex)
        {
            if (CheckCache(cellIndex, out ISaveCellContext instantResult))
                return instantResult;

            if (_cachedSaveCell != null)
                await _cachedSaveCell.SaveAllAsync();

            SavedContext savedContext = new SavedContext(cellIndex);

            await AsyncFileSystem.SaveAsync(savedContext);

            Dictionary<Type, object> cellContext = await AsyncFileSystem.LoadAllAsync(cellIndex.ToString());

            _cachedSaveCell = new SaveCellDataContext(cellContext, FileSystem, AsyncFileSystem, cellIndex);
            _cachedSaveCellIndex = cellIndex;

            return _cachedSaveCell;
        }

        private bool CheckCache(int cellIndex, out ISaveCellContext result)
        {
            result = null;

            if (cellIndex == _cachedSaveCellIndex)
            {
                result = _cachedSaveCell;
                return true;
            }

            if (cellIndex < 0)
                throw new ArgumentOutOfRangeException("Save Cell Index can not be less than 0");

            return false;
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
