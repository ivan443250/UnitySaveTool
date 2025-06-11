using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnitySaveTool
{
    public class DefaultFileSystem : IFileSystem
    {
        private IDataConverter _dataConverter;

        private Dictionary<string, Dictionary<Type, object>> _cachedDirectoryes;

        public DefaultFileSystem(IDataConverter dataConverter)
        {
            _dataConverter = dataConverter;

            _cachedDirectoryes = new();
        }

        public async Task Save(object objectToSave, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter);

            await filesCollection.Reset(objectToSave);

            if (_cachedDirectoryes.ContainsKey(path) == false)
                _cachedDirectoryes.Add(path, new());

            Type objectType = objectToSave.GetType();

            if (_cachedDirectoryes[path].ContainsKey(objectType) == false)
                _cachedDirectoryes[path].Add(objectType, null);

            _cachedDirectoryes[path][objectType] = objectToSave;
        }

        public async Task SaveAll(Dictionary<Type, object> objectsToSave, params string[] folders)
        {
            string path = GetFullPath(true, folders);

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter);

            foreach (Type type in objectsToSave.Keys) 
                await filesCollection.Reset(objectsToSave[type]);

            if (_cachedDirectoryes.ContainsKey(path) == false)
                _cachedDirectoryes.Add(path, new());

            _cachedDirectoryes[path] = objectsToSave;
        }

        public async Task<object> Load(Type objectType, params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return null;

            if (_cachedDirectoryes.ContainsKey(path) && _cachedDirectoryes[path].ContainsKey(objectType))
                return _cachedDirectoryes[path][objectType];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter);

            return await filesCollection.Get(objectType);
        }

        public async Task<Dictionary<Type, object>> LoadAll(params string[] folders)
        {
            string path = GetFullPath(false, folders);

            if (path == null)
                return new();

            if (_cachedDirectoryes.ContainsKey(path))
                return _cachedDirectoryes[path];

            IFolderFilesCollection filesCollection = await FolderMetadata.GetFilesCollection(path, _dataConverter);

            return await filesCollection.GetAll();
        }

        private string GetFullPath(bool pathMustExist, params string[] folders)
        {
            StringBuilder checkedPath = new(Application.persistentDataPath);

            foreach (string folder in folders)
            {
                checkedPath.Append("/");
                checkedPath.Append(folder);

                if (pathMustExist && (Directory.Exists(checkedPath.ToString()) == false))
                    Directory.CreateDirectory(checkedPath.ToString());
            }

            if (Directory.Exists(checkedPath.ToString()) == false)
                return null;

            return checkedPath.ToString();
        }
    }
}
