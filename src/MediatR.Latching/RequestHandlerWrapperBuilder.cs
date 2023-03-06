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

        public RequestHandlerWrapperBuilder RegisterSimpleRequestHandlers<TRequestBase, TRequestHandlerBase>(
            out ServiceDescriptor[] handlerDescriptors)
        {
            if (!_assemblies.Any())
                throw new Exception("Assemblies be provided in order to Register Request Handlers.");

            if (!typeof(TRequestHandlerBase).IsGenericType)
                throw new Exception("Until now only request handlers with only one generic parameter is supported.");

            var genericType = typeof(TRequestHandlerBase).GetGenericTypeDefinition();

            var allTypes = _assemblies
                .SelectMany(asm => asm.ExportedTypes)
                .Where(type => type.IsClass)
                .ToList();

            var relatedHandlerWithRequest = GetRelatedHandlerWithRequest(genericType, allTypes);

            handlerDescriptors = GetHandlerDescriptors<TRequestBase, TRequestHandlerBase>(relatedHandlerWithRequest);

            return this;
        }

        public RequestHandlerWrapperBuilder HandleRequests<TRequestBase, TRequestHandlerBase>(
            Expression<Action<TRequestBase, TRequestHandlerBase, IServiceProvider>> by,
            out ServiceDescriptor delegateDescriptor)
    {
        var actionDelegate = by.Compile();

        delegateDescriptor = new ServiceDescriptor(
            serviceType: actionDelegate.GetType(),
            implementationType: actionDelegate.GetType(),
            ServiceLifetime.Scoped);

        return this;
    }

        private ServiceDescriptor[] GetHandlerDescriptors<TRequestBase, TRequestHandlerBase>(List<(Type handlerType, Type baseInterface, Type requestType)> relatedHandlerWithRequest)
        {
            var handlerDescriptors = new List<ServiceDescriptor>();

            foreach (var tupleTypes in relatedHandlerWithRequest)
            {
                var requestWrapperType = CreateRequestWrapperType(tupleTypes.requestType);

                var genericHandlerType =
                    CreateGenericSimpleRequestHandler<TRequestHandlerBase, TRequestBase>(tupleTypes.handlerType, requestWrapperType, tupleTypes.requestType);

                var mediatrHandlerType = CreateMediatrHandlerType(requestWrapperType);

                var handlerDescriptor = new ServiceDescriptor(mediatrHandlerType, genericHandlerType, ServiceLifetime.Scoped);

                handlerDescriptors.Add(handlerDescriptor);
            }

            return handlerDescriptors.ToArray();
        }

        private static List<(Type handlerType, Type baseInterface, Type requestType)> GetRelatedHandlerWithRequest(Type genericType, List<Type> allTypes)
        {
            var relatedHandlerWithRequest = new List<(Type handlerType, Type baseInterface, Type requestType)>();

            foreach (var handlerType in allTypes)
            {
                var interfaces = handlerType.GetInterfaces();

                var baseInterface = interfaces
                    .Where(itf => itf.IsGenericType)
                    .SingleOrDefault(itf => itf.GetGenericTypeDefinition().Equals(genericType));

                if (baseInterface is null)
                    continue;

                var requestType = baseInterface.GenericTypeArguments[0];

                relatedHandlerWithRequest.Add((handlerType, baseInterface, requestType));
            }

            return relatedHandlerWithRequest;
        }

        private static Type CreateMediatrHandlerType(Type requestWrapperType)
        {
            return typeof(IRequestHandler<,>).MakeGenericType(requestWrapperType, typeof(Unit));
        }

        private static Type CreateGenericSimpleRequestHandler<TRequestHandler, TRequest>(Type handlerType, Type requestWrapperType, Type requestType)
        {
            return typeof(GenericSimpleRequestHandler<,,,,>)
                .MakeGenericType(handlerType, typeof(TRequestHandler), requestWrapperType, requestType, typeof(TRequest));
        }

        private static Type CreateRequestWrapperType(Type requestType)
        {
            return typeof(SimpleRequestWrapper<>).MakeGenericType(requestType);
        }
    }
}
