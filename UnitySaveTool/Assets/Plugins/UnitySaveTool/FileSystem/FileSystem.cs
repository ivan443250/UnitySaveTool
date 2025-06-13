using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnitySaveTool
{
    public class FileSystem : IFileSystem, IAsyncFileSystem
    {
        private readonly string _globalPath;

        private IDataConverter _dataConverter;

        private Dictionary<string, Dictionary<Type, object>> _cachedDirectoryes;

        public FileSystem(IDataConverter dataConverter)
        {
            _dataConverter = dataConverter;

            _cachedDirectoryes = new();

            _globalPath = Application.persistentDataPath + "/UnitySaveTool";
        }

        public FileSystem(IDataConverter dataConverter, string globalPath) : this(dataConverter) 
        {
            _globalPath = globalPath;
        }

        #region SaveMethods

        public void Save(object objectToSave, params string[] folders)
        {
            _ = SaveInternal(objectToSave, false, folders);
        }

        public async UniTask SaveAsync(object objectToSave, params string[] folders)
        {
            await SaveInternal(objectToSave, true, folders);
        }

        private async UniTask SaveInternal(object objectToSave, bool doAsync, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter, doAsync);

            if (doAsync)
                await filesCollection.ResetAsync(objectToSave);
            else
                filesCollection.Reset(objectToSave);

            if (_cachedDirectoryes.ContainsKey(path) == false)
                _cachedDirectoryes.Add(path, new());

            Type objectType = objectToSave.GetType();

            if (_cachedDirectoryes[path].ContainsKey(objectType) == false)
                _cachedDirectoryes[path].Add(objectType, null);

            _cachedDirectoryes[path][objectType] = objectToSave;
        }

        #endregion

        #region SaveAllMethods

        public void SaveAll(Dictionary<Type, object> objectsToSave, params string[] folders)
        {
            _ = SaveInternal(objectsToSave, false, folders);
        }

        public async UniTask SaveAllAsync(Dictionary<Type, object> objectsToSave, params string[] folders)
        {
            await SaveAllInternal(objectsToSave, true, folders);
        }

        private async UniTask SaveAllInternal(Dictionary<Type, object> objectsToSave, bool doAsync, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter, doAsync);

            if (doAsync)
                foreach (Type type in objectsToSave.Keys)
                    await filesCollection.ResetAsync(objectsToSave[type]);
            else
                foreach (Type type in objectsToSave.Keys)
                    filesCollection.Reset(objectsToSave[type]);

            if (_cachedDirectoryes.ContainsKey(path) == false)
                _cachedDirectoryes.Add(path, new());

            _cachedDirectoryes[path] = objectsToSave;
        }

        #endregion

        #region LoadMethods

        public object Load(Type objectType, params string[] folders)
        {
            return LoadInternal(objectType, false, folders);
        }

        public async UniTask<object> LoadAsync(Type objectType, params string[] folders)
        {
            return await LoadInternal(objectType, true, folders);
        }

        public async UniTask<object> LoadInternal(Type objectType, bool doAsync, params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return null;

            if (_cachedDirectoryes.ContainsKey(path) && _cachedDirectoryes[path].ContainsKey(objectType))
                return _cachedDirectoryes[path][objectType];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter, doAsync);

            return doAsync ? await filesCollection.GetAsync(objectType) : filesCollection.Get(objectType);
        }

        #endregion

        #region LoadAllMethods

        public Dictionary<Type, object> LoadAll(params string[] folders)
        {
            return LoadAllInternal(false, folders).GetAwaiter().GetResult();
        }

        public async UniTask<Dictionary<Type, object>> LoadAllAsync(params string[] folders)
        {
            return await LoadAllInternal(true, folders);
        }

        private async UniTask<Dictionary<Type, object>> LoadAllInternal(bool doAsync, params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return new();

            if (_cachedDirectoryes.ContainsKey(path))
                return _cachedDirectoryes[path];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter, doAsync);

            return doAsync ? await filesCollection.GetAllAsync() : filesCollection.GetAll();
        }

        #endregion

        private string GetFullPath(bool pathMustExist, params string[] folders)
        {
            if (Directory.Exists(_globalPath) == false)
                Directory.CreateDirectory(_globalPath);

            StringBuilder checkedPath = new(_globalPath);

            foreach (string folder in folders)
            {
                checkedPath.Append("/");
                checkedPath.Append(folder);

                string checkedPathString = checkedPath.ToString();

                if (pathMustExist && (Directory.Exists(checkedPathString) == false))
                    Directory.CreateDirectory(checkedPathString);
            }

            if (Directory.Exists(checkedPath.ToString()) == false)
                return null;

            return checkedPath.ToString();
        }
    }
}
