using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using InterWebs.Domain.Model;
using InterWebs.Domain.Repository;
using InterWebs.Persistence.Repository;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;

namespace InterWebs
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            var container = BuildUnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IPersistenceOrientedRepository<ChatMessage>, MongoRepository<ChatMessage>>();
            container.RegisterType<IPersistenceOrientedRepository<User>, MongoRepository<User>>();

            return container;
        }
    }

    public class ScopeContainer : IDependencyScope
    {
        protected IUnityContainer Container;

        public ScopeContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            Container = container;
        }

        public object GetService(Type serviceType)
        {
            return Container.IsRegistered(serviceType)
                ? Container.Resolve(serviceType)
                : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.IsRegistered(serviceType)
                ? Container.ResolveAll(serviceType)
                : new List<object>();
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }

    class IoCContainer : ScopeContainer, System.Web.Http.Dependencies.IDependencyResolver
    {
        public IoCContainer(IUnityContainer container)
            : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            var child = Container.CreateChildContainer();
            return new ScopeContainer(child);
        }
    }
}