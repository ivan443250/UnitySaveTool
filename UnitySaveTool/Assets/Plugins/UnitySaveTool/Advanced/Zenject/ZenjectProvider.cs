using System;
using System.Collections.Generic;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class ZenjectProvider : Zenject.IProvider
    {
        public bool TypeVariesBasedOnMemberType => true;

        public bool IsCached => true;

        private UnitySaveTool.IProvider _provider;
        private DiContainer _container;

        public ZenjectProvider(UnitySaveTool.IProvider provider, DiContainer container)
        {
            _provider = provider;
            _container = container;
        }

        public void GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Type type = context.MemberType;

            if (type.IsDefined(typeof(SaveToolDataAttribute), false) == false || _provider.HasInstance(type) == false)
            {
                buffer.Clear();
                injectAction = null;
                return;
            }

            object instance = _provider.GetInstance(type);
            buffer.Add(instance);

            injectAction = () =>
            {
                if (instance != null)
                {
                    context.Container.Inject(instance);
                }
            };
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _provider.GetInstanceType(context.MemberType);
        }
    }
}
