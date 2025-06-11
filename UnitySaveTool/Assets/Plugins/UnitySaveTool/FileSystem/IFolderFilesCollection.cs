using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IFolderFilesCollection
    {
        Task Set(object obj);
        Task Reset(object obj);

        Task Remove(Type type);
        Task ClearAll();

        Task<object> Get(Type type);
        Task<Dictionary<Type, object>> GetAll();

        bool HasType(Type type);
    }
}
