using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IFolderFilesCollection
    {
       UniTask Set(object obj);
       UniTask Reset(object obj);

       UniTask Remove(Type type);
       UniTask ClearAll();

       UniTask<object> Get(Type type);
       UniTask<Dictionary<Type, object>> GetAll();

        bool HasType(Type type);
    }
}
