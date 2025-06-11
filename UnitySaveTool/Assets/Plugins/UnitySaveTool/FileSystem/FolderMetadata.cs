using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async static Task<IFolderFilesCollection> GetFilesCollection(string folderPath, IDataConverter dataConverter)
        {
            FolderMetadata folderMetadata;

            string path = GetFullPath(folderPath, typeof(FolderMetadata));

            if (File.Exists(path) == false)
            {
                folderMetadata = new(folderPath);
            }
            else
            {
                string metadataString = await File.ReadAllTextAsync(path);

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

        public async Task Set(object obj)
        {
            Type type = obj.GetType();

            if (type == typeof(FolderMetadata))
                throw new Exception();

            string path = GetFullPath(type);

            if (_types.Contains(type))
                throw new Exception();

            await File.WriteAllTextAsync(path, _dataConverter.ConvertFromObject(obj));

            _types.Add(type);

            await Save();
        }

        public async Task Reset(object obj)
        {
            await Remove(obj.GetType());

            await Set(obj);
        }

        public async Task Remove(Type type)
        {
            string path = GetFullPath(type);

            if (_types.Contains(type) == false)
                return;

            File.Delete(path);
            _types.Remove(type);

            await Save();
        }

        public async Task ClearAll()
        {
            foreach (Type type in _types)
                await Remove(type);
        }

        public bool HasType(Type type)
        {
            return _types.Contains(type);
        }

        public async Task<object> Get(Type type)
        {
            if (_types.Contains(type) == false)
                return null;

            string path = GetFullPath(type);

            string objectString = await File.ReadAllTextAsync(path);

            return _dataConverter.ConvertToObject(objectString, type);
        }

        public async Task<Dictionary<Type, object>> GetAll()
        {
            Dictionary<Type, object> deserializedObjects = new();

            foreach (Type type in _types)
                deserializedObjects.Add(type, await Get(type));

            return deserializedObjects;
        }

        private async Task Save()
        {
            string path = GetFullPath(typeof(FolderMetadata));

            await File.WriteAllTextAsync(path, _dataConverter.ConvertFromObject(this));
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
