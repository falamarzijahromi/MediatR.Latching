using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MediatR.Latching
{
    public static class SerivceCollectionExtensions
    {
        public static IServiceCollection LatchRequestHandlers<TRequestHandlerBase, TRequestBase>(
            this IServiceCollection services,
            Expression<Action<TRequestBase, TRequestHandlerBase, IServiceProvider>> by,
            params Assembly[] lookInto)
        {
            var builder = new RequestHandlerWrapperBuilder();

            builder
               .LookIntoAssemblies(assemblies: lookInto)
               .RegisterSimpleRequestHandlers<TRequestBase, TRequestHandlerBase>(out var handlerDescriptors)
               .HandleRequests(by, out var delegateDescriptor);

            services.Add(delegateDescriptor);

            foreach (var descriptor in handlerDescriptors)
            {
                services.Add(descriptor);
            }

            return services;
        }
    }
}
