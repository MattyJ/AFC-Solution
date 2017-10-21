using System;
using System.Collections.Generic;
using Fujitsu.AFC.Core.Interfaces;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Core.Injection
{
    public class ObjectBuilder : IObjectBuilder
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            //TODO: Investigate SharePoint injection issue
            // An attempt to override an existing mapping was detected for type Microsoft.SharePoint.Client.IScriptTypeFactory with name "", 
            // currently mapped to type Microsoft.Online.SharePoint.Client.TenantAdmin.ScriptTypeFactory, to type Microsoft.SharePoint.Client.ScriptTypeFactory.

            //container.RegisterTypes(
            //    AllClasses.FromAssembliesInBasePath(),
            //    WithMappings.FromMatchingInterface,
            //    WithName.Default,
            //    WithLifetime.PerResolve);

            return container;
        });

        private readonly Stack<IUnityContainer> _childContainers = new Stack<IUnityContainer>();

        public ObjectBuilder()
        {
        }

        public ObjectBuilder(params Action<IUnityContainer>[] registers)
        {
            foreach (var register in registers)
            {
                register(Container.Value);
            }
        }

        public void AddChildContainer()
        {
            _childContainers.Push(Container.Value.CreateChildContainer());
        }

        public void RemoveChildContainer()
        {
            if (_childContainers.Count > 0)
            {
                var container = _childContainers.Pop();
                container.Dispose();
            }
        }

        public void RemoveAllChildContainers()
        {
            while (_childContainers.Count > 0)
            {
                var container = _childContainers.Pop();
                container.Dispose();
            }
        }

        public IUnityContainer GetContainer()
        {
            return Container.Value;
        }

        public T Resolve<T>()
        {
            if (_childContainers.Count > 0)
            {
                var container = _childContainers.Peek();
                return container.Resolve<T>();
            }
            return Container.Value.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return Resolve<T>(name, false);
        }

        public T Resolve<T>(string name, bool allowDefault)
        {
            var container = Container.Value;
            if (_childContainers.Count > 0)
            {
                container = _childContainers.Peek();
            }
            if (container.IsRegistered<T>(name))
            {
                return container.Resolve<T>(name);
            }
            if (allowDefault && container.IsRegistered<T>())
            {
                return container.Resolve<T>();
            }
            return container.Resolve<T>(name);
        }
    }

}
