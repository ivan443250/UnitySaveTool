using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface IFolderFilesCollection
    {
        void Set(object obj);
        UniTask SetAsync(object obj);

        void Reset(object obj);
        UniTask ResetAsync(object obj);

        void Remove(Type type);
        UniTask RemoveAsync(Type type);

        void ClearAll();
        UniTask ClearAllAsync();

        object Get(Type type);
        UniTask<object> GetAsync(Type type);

        Dictionary<Type, object> GetAll();
        UniTask<Dictionary<Type, object>> GetAllAsync();

        bool HasType(Type type);
    }
}
