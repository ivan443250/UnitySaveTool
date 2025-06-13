using System;
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
            _diContainer.RegisterProvider(new BindingId(typeof(object), null), null, new ZenjectProvider(provider, _diContainer), true);
        }

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _diContainer.Bind<TInterface>().FromInstance(instance).AsSingle();
        }
    }
}