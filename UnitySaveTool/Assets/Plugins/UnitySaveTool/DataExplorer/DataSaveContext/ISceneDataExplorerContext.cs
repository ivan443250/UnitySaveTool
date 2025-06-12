using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public interface ISceneDataExplorerContext
    {
        object GetData(Type type, bool createIfNot = false);
        T GetData<T>(bool createIfNot = false) where T : class;

        HashSet<Type> GetAllDataTypes();

        UniTask Save<T>(T data) where T : class;
        UniTask SaveAll();
    }
}
