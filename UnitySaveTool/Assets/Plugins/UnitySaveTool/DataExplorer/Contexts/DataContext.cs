using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitySaveTool
{
    public abstract class DataContext : ISceneDataContext
    {
        private readonly Dictionary<Type, object> _contextData;

        protected readonly IFileSystem FileSystem;
        protected readonly IAsyncFileSystem AsyncFileSystem;

        protected readonly string[] Folders;

        protected abstract string ContextName { get; }
        protected abstract SaveContext ContextType { get; }

        public DataContext(Dictionary<Type, object> contextData,
            IFileSystem fileSystem,
            IAsyncFileSystem asyncFileSystem,
            params string[] folders)
        {
            _contextData = contextData;

            FileSystem = fileSystem;
            AsyncFileSystem = asyncFileSystem;

            Folders = folders;
        }

        public bool CheckTypeAddCondition(Type type)
        {
            SaveToolDataAttribute saveAttribute = type.GetCustomAttribute<SaveToolDataAttribute>();

            if (type.GetConstructor(Type.EmptyTypes) == null)
                throw new InvalidOperationException($"Type {type} can not be added because type " +
                    $"dont contain empty constructor");

            if (saveAttribute == null)
                throw new InvalidOperationException($"Type {type} can not be added in " +
                    $"{ContextName} because it dont contain SaveToolDataAttribute");

            if (saveAttribute.Context != ContextType)
                throw new InvalidOperationException($"Type {type} can not be added in " +
                    $"{ContextName} because Context value in SaveToolDataAttribute is not {ContextType}");

            return true;
        }

        public HashSet<Type> GetAllDataTypes()
        {
            return new(_contextData.Keys);
        }

        public object GetData(Type type, bool createIfNot = false)
        {
            return GetDataInternal(type, createIfNot);
        }

        public T GetData<T>(bool createIfNot = false) where T : class
        {
            return GetDataInternal(typeof(T), createIfNot) as T;
        }

        public void Save<T>(T data) where T : class
        {
            CheckTypeAddCondition(typeof(T));

            SaveDataInternal(data);

            FileSystem.Save(data, Folders);
        }

        public void SaveAll()
        {
            FileSystem.SaveAll(_contextData, Folders);
        }

        public async UniTask SaveAllAsync()
        {
            await AsyncFileSystem.SaveAllAsync(_contextData, Folders);
        }

        public async UniTask SaveAsync<T>(T data) where T : class
        {
            CheckTypeAddCondition(typeof(T));

            SaveDataInternal(data);

            await AsyncFileSystem.SaveAsync(data, Folders);
        }

        private object GetDataInternal(Type type, bool createIfNot = false)
        {
            if (_contextData.ContainsKey(type) == false && (type.GetConstructor(Type.EmptyTypes) == null || createIfNot == false))
                return null;

            if (_contextData.ContainsKey(type) == false)
                _contextData.Add(type, Activator.CreateInstance(type));

            return _contextData[type];
        }

        private void SaveDataInternal(object data)
        {
            Type dataType = data.GetType();

            if (_contextData.ContainsKey(dataType) == false)
                _contextData.Add(dataType, null);

            _contextData[dataType] = data;
        }
    }
}
