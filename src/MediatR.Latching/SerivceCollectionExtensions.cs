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
            Expression<Action<TRequestHandlerBase, TRequestBase>> by,
            params Assembly[] lookInto)
        {
            var builder = new RequestHandlerWrapperBuilder();

            var allDescriptors = builder
               .LookIntoAssemblies(assemblies: lookInto)
               .HandleRequests(by);

            foreach (var descriptor in allDescriptors)
            {
                services.Add(descriptor);
            }

            return services;
        }
    }
}
