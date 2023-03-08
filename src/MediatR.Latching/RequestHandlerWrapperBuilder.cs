using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MediatR.Latching
{
    public class RequestHandlerWrapperBuilder
    {
        private Assembly[] _assemblies;

        public RequestHandlerWrapperBuilder LookIntoAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies;

            return this;
        }

        public ServiceDescriptor[] HandleRequests<TRequestHandlerBase, TRequestBase>(
            Expression<Action<TRequestHandlerBase, TRequestBase>> by)
        {
            if (!_assemblies.Any())
                throw new Exception("Assemblies be provided in order to Register Request Handlers.");

            if (!typeof(TRequestHandlerBase).IsGenericType)
                throw new Exception("Until now only request handlers with only one generic parameter is supported.");

            var genericType = typeof(TRequestHandlerBase).GetGenericTypeDefinition();

            var byBody = by.Body as MethodCallExpression;

            if (byBody == null || byBody.Method.GetParameters().Length != 1)
            {
                throw new Exception("Handling expression must be a single method call lambda with only one parameter.");
            }

            var allClassesWithInterfaces = _assemblies
                .SelectMany(asm => asm.ExportedTypes)
                .Where(type => type.IsClass)
                .Select(type => new { RequestHandlerType = type, Interfaces = type.GetInterfaces().Where(i => i.IsGenericType) })
                .Where(map => map.Interfaces.Any())
                .ToList();

            var allDescriptors = new List<ServiceDescriptor>();

            foreach (var @classWithInterfaces in allClassesWithInterfaces)
            {
                var handlingInterface = @classWithInterfaces.Interfaces
                    .Where(i => i.GetGenericTypeDefinition().Equals(genericType))
                    .SingleOrDefault();

                if (handlingInterface is null)
                {
                    continue;
                }

                var requestType = handlingInterface.GenericTypeArguments[0];
                var requestWrapperType = CreateRequestWrapperType(requestType);

                var requestHandlerDelegateDescriptor = CreateHandlingDelegateServiceDescriptor(
                    handlingMethodName: byBody.Method.Name,
                    requestHandlerType: classWithInterfaces.RequestHandlerType,
                    requestType: requestType);

                var mediatrRequestHandlerDescriptor = CreateMediatrRequestHandlerDescriptor(
                    requestWrapperType: requestWrapperType,
                    requestHandlerType: classWithInterfaces.RequestHandlerType,
                    requestType: requestType);

                var requestHandlerDescriptor = new ServiceDescriptor(
                    serviceType: classWithInterfaces.RequestHandlerType,
                    implementationType: classWithInterfaces.RequestHandlerType,
                    lifetime: ServiceLifetime.Scoped);


                allDescriptors.Add(mediatrRequestHandlerDescriptor);
                allDescriptors.Add(requestHandlerDelegateDescriptor);
                allDescriptors.Add(requestHandlerDescriptor);
            }

            return allDescriptors.ToArray();
        }

        private static Type CreateRequestWrapperType(Type requestType)
        {
            return typeof(SimpleRequestWrapper<>).MakeGenericType(requestType);
        }

        private static ServiceDescriptor CreateMediatrRequestHandlerDescriptor(
            Type requestWrapperType,
            Type requestHandlerType,
            Type requestType)
        {
            var mediatrRequestHandlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(requestWrapperType, typeof(Unit));

            var mediatrRequestHandlerService = typeof(GenericSimpleRequestHandler<,,>)
                .MakeGenericType(requestHandlerType, requestWrapperType, requestType);

            return new ServiceDescriptor(
               serviceType: mediatrRequestHandlerType,
               implementationType: mediatrRequestHandlerService,
               lifetime: ServiceLifetime.Scoped);
        }

        private static ServiceDescriptor CreateHandlingDelegateServiceDescriptor(
            string handlingMethodName,
            Type requestHandlerType,
            Type requestType)
        {
            var handlingMethod = requestHandlerType.GetMethod(handlingMethodName);

            var handlerParameter = Expression.Parameter(requestHandlerType, "handler");
            var requestParameter = Expression.Parameter(requestType, "request");

            var handlingMethodExpression = Expression.Call(handlerParameter, handlingMethod, requestParameter);

            var handlingActionType = typeof(Action<,>).MakeGenericType(requestHandlerType, requestType);

            var handlingDelegate =
                Expression.Lambda(handlingActionType, handlingMethodExpression, handlerParameter, requestParameter).Compile();

            return new ServiceDescriptor(handlingActionType, handlingDelegate);
        }
    }
}
