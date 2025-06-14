using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public interface IDataContext
    {
        HashSet<Type> GetAllDataTypes();

        object GetData(Type type, bool createIfNot = false);
        T GetData<T>(bool createIfNot = false) where T : class;

        bool CheckTypeAddCondition(Type type);
    }
}
