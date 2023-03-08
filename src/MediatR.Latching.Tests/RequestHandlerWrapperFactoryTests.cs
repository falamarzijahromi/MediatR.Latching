using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Latching.Tests
{
    public class RequestHandlerWrapperFactoryTests
    {
        private readonly RequestHandlerWrapperBuilder _builder;

        public RequestHandlerWrapperFactoryTests()
        {
            _builder = new RequestHandlerWrapperBuilder();
        }

        [Fact]
        public void Simple_Request_Abstraction_Correct_Simple_Request_Handlers_Must_Be_Found()
        {
            var mediatrRequestHandlerType = typeof(IRequestHandler<SimpleRequestWrapper<SimpleRequest>, Unit>);
            var genericRequestHandler = typeof(
                GenericSimpleRequestHandler<SimpleRequestHandler, SimpleRequestWrapper<SimpleRequest>, SimpleRequest>);

            var descriptors = _builder
               .LookIntoAssemblies(assemblies: this.GetType().Assembly)
               .HandleRequests<ISimpleRequestHandler<ISimpleRequest>, ISimpleRequest>(
                by: (handler, request) => handler.ShouldHandle(request));

            var mediatrRequestHandlerDescriptor = Assert
                .Single<ServiceDescriptor>(descriptors, desc => desc.ServiceType.Equals(mediatrRequestHandlerType));

            Assert.Equal(genericRequestHandler, mediatrRequestHandlerDescriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Scoped, mediatrRequestHandlerDescriptor.Lifetime);
        }

        [Fact]
        public void Simple_Request_Handler_Descriptor_Must_Be_Created()
        {
            var handlerType = typeof(SimpleRequestHandler);

            var descriptors = _builder
               .LookIntoAssemblies(assemblies: this.GetType().Assembly)
               .HandleRequests<ISimpleRequestHandler<ISimpleRequest>, ISimpleRequest>(
                by: (handler, request) => handler.ShouldHandle(request));


            var requestHandlerDescriptor = Assert
                .Single<ServiceDescriptor>(descriptors, desc => desc.ServiceType.Equals(handlerType));

            Assert.Equal(handlerType, requestHandlerDescriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Scoped, requestHandlerDescriptor.Lifetime);
        }

        [Fact]
        public void Simple_Request_Handler_Delegate_Descriptor_Must_Be_Created()
        {
            var delegateType = typeof(Action<,>)
                .MakeGenericType(typeof(SimpleRequestHandler), typeof(SimpleRequest));

            var descriptors = _builder
               .LookIntoAssemblies(assemblies: this.GetType().Assembly)
               .HandleRequests<ISimpleRequestHandler<ISimpleRequest>, ISimpleRequest>(
                by: (handler, request) => handler.ShouldHandle(request));

            var delegateDescriptor = Assert
                .Single<ServiceDescriptor>(descriptors, desc => desc.Lifetime == ServiceLifetime.Singleton);

            Assert.NotNull(delegateDescriptor);
            Assert.NotNull(delegateDescriptor.ImplementationInstance);

            Assert.Equal(delegateType, delegateDescriptor.ServiceType);
        }
    }
}