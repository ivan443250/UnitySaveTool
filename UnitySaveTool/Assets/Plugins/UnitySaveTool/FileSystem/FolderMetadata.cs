using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySaveTool.Tools;

namespace UnitySaveTool
{
    [Serializable]
    public sealed class FolderMetadata : ISerializationCallbackReceiver, IFolderFilesCollection
    {
        [SerializeField] private SerializableType[] _serializableTypes;
        [SerializeField] private string _folderPath;

        private HashSet<Type> _types;
        private IDataConverter _dataConverter;

        public async static UniTask<IFolderFilesCollection> GetFilesCollection(string folderPath, IDataConverter dataConverter, bool doAsync)
        {
            FolderMetadata folderMetadata;

            string path = GetFullPath(folderPath, typeof(FolderMetadata));

            if (File.Exists(path) == false)
            {
                folderMetadata = new(folderPath);
            }
            else
            {
                string metadataString = doAsync ? await File.ReadAllTextAsync(path) : File.ReadAllText(path);

                object metadataObj = dataConverter.ConvertToObject(metadataString, typeof(FolderMetadata));

                if (metadataObj is not FolderMetadata folderMetadataExample)
                    throw new Exception();

                folderMetadata = folderMetadataExample;
            }

            folderMetadata._dataConverter = dataConverter;

            return folderMetadata;
        }

        public FolderMetadata() { }

        private FolderMetadata(string folderPath)
        {
            _folderPath = folderPath;
            _types = new();
        }

        public void OnAfterDeserialize()
        {
            _types = new();

            foreach (SerializableType serializableType in _serializableTypes)
                _types.Add(serializableType.GetValue());
        }

        public void OnBeforeSerialize()
        {
            _serializableTypes = _types.Select(t => new SerializableType(t)).ToArray();
        }

        #region SetMethods

        public void Set(object obj)
        {
            _ = SetInternal(obj, false);
        }

        public async UniTask SetAsync(object obj)
        {
            await SetInternal(obj, true);
        }

        private async UniTask SetInternal(object obj, bool doAsync)
        {
            Type type = obj.GetType();

            if (type == typeof(FolderMetadata))
                throw new Exception();

            string path = GetFullPath(type);

            if (_types.Contains(type))
                throw new Exception();

            if (doAsync)
                await File.WriteAllTextAsync(path, _dataConverter.ConvertFromObject(obj));
            else
                File.WriteAllText(path, _dataConverter.ConvertFromObject(obj));

            _types.Add(type);

            if (doAsync)
                await SaveAsync();
            else
                Save();
        }

        #endregion

        #region ResetMethods

        public void Reset(object obj)
        {
            _ = ResetInternal(obj, false);
        }

        public async UniTask ResetAsync(object obj)
        {
            await ResetInternal(obj, true);    
        }

        private async UniTask ResetInternal(object obj, bool doAsync)
        {
            if (doAsync)
            {
                await RemoveAsync(obj.GetType());
                await SetAsync(obj);
            }
            else
            {
                Remove(obj.GetType());
                Set(obj);
            }
        }

        #endregion

        #region RemoveMethods

        public void Remove(Type type)
        {
            _ = RemoveInternal(type, false);
        }

        public async UniTask RemoveAsync(Type type)
        {
            await RemoveInternal(type, true);
        }

        private async UniTask RemoveInternal(Type type, bool doAsync)
        {
            string path = GetFullPath(type);

            if (_types.Contains(type) == false)
                return;

            File.Delete(path);
            _types.Remove(type);

            if (doAsync) 
                await SaveAsync();
            else 
                Save();
        }

        #endregion

        #region ClearAllMethods

        public async UniTask ClearAllAsync()
        {
            foreach (Type type in _types)
                await RemoveAsync(type);
        }

        public void ClearAll()
        {
            foreach (Type type in _types)
                Remove(type);
        }

        #endregion

        #region GetMethods

        public object Get(Type type)
        {
            return GetInternal(type, false).GetAwaiter().GetResult();
        }

        public async UniTask<object> GetAsync(Type type)
        {
            return await GetInternal(type, true);
        }

        private async UniTask<object> GetInternal(Type type, bool doAsync)
        {
            if (_types.Contains(type) == false)
                return null;

            string path = GetFullPath(type);

            string objectString = doAsync ? await File.ReadAllTextAsync(path) : File.ReadAllText(path);

            return _dataConverter.ConvertToObject(objectString, type);
        }

        #endregion

        #region GetAllMethods

        public Dictionary<Type, object> GetAll()
        {
            return GetAllInternal(false).GetAwaiter().GetResult();
        }

        public async UniTask<Dictionary<Type, object>> GetAllAsync()
        {
            return await GetAllInternal(true);
        }

        private async UniTask<Dictionary<Type, object>> GetAllInternal(bool doAsync)
        {
            Dictionary<Type, object> deserializedObjects = new();

            foreach (Type type in _types)
                deserializedObjects.Add(type, doAsync ? await GetAsync(type) : Get(type));

            return deserializedObjects;
        }

        #endregion

        public bool HasType(Type type)
        {
            return _types.Contains(type);
        }

        private async UniTask SaveAsync()
        {
            string path = GetFullPath(typeof(FolderMetadata));

            await File.WriteAllTextAsync(path, _dataConverter.ConvertFromObject(this));
        }

        private void Save()
        {
            string path = GetFullPath(typeof(FolderMetadata));

            File.WriteAllText(path, _dataConverter.ConvertFromObject(this));
        }

        private string GetFullPath(Type type)
        {
            return GetFullPath(_folderPath, type);
        }

        private static string GetFullPath(string folderPath, Type type)
        {
            return $"{folderPath}/{type.FullName}.json";
        }
    }
}
