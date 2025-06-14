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
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = FolderMetadata.GetFilesCollection(path, _dataConverter);

            filesCollection.Reset(objectToSave);

            SaveInternal(objectToSave, path);
        }

        public async UniTask SaveAsync(object objectToSave, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollectionAsync(path, _dataConverter);

            await filesCollection.ResetAsync(objectToSave);

            SaveInternal(objectToSave, path);
        }

        private void SaveInternal(object objectToSave, string path)
        {
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
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = FolderMetadata.GetFilesCollection(path, _dataConverter);

            foreach (Type type in objectsToSave.Keys)
                filesCollection.Reset(objectsToSave[type]);

            SaveAllInternal(objectsToSave, path);
        }

        public async UniTask SaveAllAsync(Dictionary<Type, object> objectsToSave, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollectionAsync(path, _dataConverter);

            foreach (Type type in objectsToSave.Keys)
                await filesCollection.ResetAsync(objectsToSave[type]);

            SaveAllInternal(objectsToSave, path);
        }

        private void SaveAllInternal(Dictionary<Type, object> objectsToSave, string path)
        {
            if (_cachedDirectoryes.ContainsKey(path) == false)
                _cachedDirectoryes.Add(path, new());

            _cachedDirectoryes[path] = objectsToSave;
        }

        #endregion

        #region LoadMethods

        public object Load(Type objectType, params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return null;

            if (_cachedDirectoryes.ContainsKey(path) && _cachedDirectoryes[path].ContainsKey(objectType))
                return _cachedDirectoryes[path][objectType];

            IFolderFilesCollection filesCollection = FolderMetadata.GetFilesCollection(path, _dataConverter);

            return filesCollection.Get(objectType);
        }

        public async UniTask<object> LoadAsync(Type objectType, params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return null;

            if (_cachedDirectoryes.ContainsKey(path) && _cachedDirectoryes[path].ContainsKey(objectType))
                return _cachedDirectoryes[path][objectType];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollectionAsync(path, _dataConverter);

            return await filesCollection.GetAsync(objectType);
        }

        #endregion

        #region LoadAllMethods

        public Dictionary<Type, object> LoadAll(params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return new();

            if (_cachedDirectoryes.ContainsKey(path))
                return _cachedDirectoryes[path];

            IFolderFilesCollection filesCollection = FolderMetadata.GetFilesCollection(path, _dataConverter);

            return filesCollection.GetAll();
        }

        public async UniTask<Dictionary<Type, object>> LoadAllAsync(params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return new();

            if (_cachedDirectoryes.ContainsKey(path))
                return _cachedDirectoryes[path];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollectionAsync(path, _dataConverter);

            return await filesCollection.GetAllAsync();
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
