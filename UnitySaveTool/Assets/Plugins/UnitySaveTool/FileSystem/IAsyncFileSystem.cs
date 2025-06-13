using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public interface IAsyncFileSystem
    {
        UniTask SaveAsync(object objectToSave, params string[] folders);
        UniTask SaveAllAsync(Dictionary<Type, object> objectsToSave, params string[] folders);

        UniTask<object> LoadAsync(Type objectType, params string[] folders);
        UniTask<Dictionary<Type, object>> LoadAllAsync(params string[] folders);
    }
}