using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public interface IFileSystem
    {
        void Save(object objectToSave, params string[] folders);
        void SaveAll(Dictionary<Type, object> objectsToSave, params string[] folders);

        object Load(Type objectType, params string[] folders);
        Dictionary<Type, object> LoadAll(params string[] folders);
    }
}