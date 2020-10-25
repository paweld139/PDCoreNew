﻿using Autofac;
using CommonServiceLocator;
using System;
using System.Collections.Generic;

namespace PDCore.Helpers
{
    public class AutofacServiceLocator : IServiceLocator
    {
        private readonly IContainer container;

        public AutofacServiceLocator(IContainer container)
        {
            this.container = container;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var collectionType = typeof(IEnumerable<>);

            var closedType = collectionType.MakeGenericType(serviceType);

            return (IEnumerable<object>)container.Resolve(closedType);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return container.Resolve<IEnumerable<TService>>();
        }

        public object GetInstance(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return container.ResolveNamed(key, serviceType);
        }

        public TService GetInstance<TService>()
        {
            TService result = default(TService);

            if (container.IsRegistered<TService>())
                result = container.Resolve<TService>();

            return result;
        }

        public TService GetInstance<TService>(string key)
        {
            return container.ResolveNamed<TService>(key);
        }

        public object GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }
    }
}
