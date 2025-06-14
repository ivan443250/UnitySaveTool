using System;
using UnitySaveTool.Tools;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class ZenjectDIContainer : IDIContainer
    {
        private DiContainer _diContainer;

        public ZenjectDIContainer(DiContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _diContainer = container;
        }

        public bool HasBinding(Type type)
        {
            return _diContainer.HasBinding(type);
        }

        public void RegisterProvdier(IProvider provider)
        {
            foreach (Type type in AttributeTypeFinder.GetTypesWithAttribute<SaveToolDataAttribute>())
            {
                if (provider.HasInstance(type) == false)
                    throw new InvalidOperationException();

                _diContainer.Bind(type).FromInstance(provider.GetInstance(type));
            }
        }

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _diContainer.Bind<TInterface>().FromInstance(instance).AsSingle();
        }
    }
}