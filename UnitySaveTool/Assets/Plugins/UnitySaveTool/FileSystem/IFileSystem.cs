using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IFileSystem
    {
       UniTask Save(object objectToSave, params string[] folders);
       UniTask SaveAll(Dictionary<Type, object> objectsToSave, params string[] folders);

       UniTask<object> Load(Type objectType, params string[] folders);
       UniTask<Dictionary<Type, object>> LoadAll(params string[] folders);
    }
}