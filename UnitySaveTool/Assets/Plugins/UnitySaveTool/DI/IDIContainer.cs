using System;

namespace UnitySaveTool
{
    public interface IDIContainer
    {
        void RegisterProvdier(IProvider provider);
        void RegisterInstance<TInterface>(TInterface instance);
        bool HasBinding(Type type);
    }
}
