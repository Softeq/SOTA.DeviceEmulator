using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using EnsureThat;
using MediatR;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public static class ToggleableMediatorServiceFactory
    {
        public static ServiceFactory Create(IComponentContext context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            var c = context.Resolve<IComponentContext>();

            object Resolve(Type type)
            {
                var instance = c.Resolve(type);
                return Filter((dynamic) instance);
            }

            return Resolve;
        }

        private static object Filter<T>(IEnumerable<T> instances)
        {
            return instances.Where(i =>
            {
                if (i is IToggleable toggleable)
                {
                    return toggleable.IsEnabled;
                }

                return true;
            });
        }

        private static object Filter(object instance)
        {
            return instance;
        }
    }
}
