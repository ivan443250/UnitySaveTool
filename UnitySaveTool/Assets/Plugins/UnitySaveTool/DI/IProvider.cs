using System;

namespace UnitySaveTool
{
    public interface IProvider
    {
        object GetInstance(Type typeToResolve);
        bool HasInstance(Type typeToResolve);
        Type GetInstanceType(Type typeToResolve);
    }
}
