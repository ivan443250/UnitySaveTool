using System;

namespace UnitySaveTool
{
    public interface IDIContainer
    {
        void InstallProvdier(IProvider provider);
        void RegisterInstance<TInterface>(TInterface instance);
        bool HasBinding(Type type);
        T Resolve<T>() where T : class;
    }
}
