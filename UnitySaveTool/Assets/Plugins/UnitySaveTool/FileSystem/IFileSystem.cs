using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IFileSystem
    {
        Task Save(object objectToSave, params string[] folders);
        Task SaveAll(Dictionary<Type, object> objectsToSave, params string[] folders);

        Task<object> Load(Type objectType, params string[] folders);
        Task<Dictionary<Type, object>> LoadAll(params string[] folders);
    }
}