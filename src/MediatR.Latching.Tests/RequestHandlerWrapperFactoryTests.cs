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
                GenericSimpleRequestHandler<SimpleRequestHandler, ISimpleRequestHandler<ISimpleRequest>, SimpleRequestWrapper<SimpleRequest>, SimpleRequest, ISimpleRequest>);

            _builder
               .LookIntoAssemblies(assemblies: this.GetType().Assembly)
               .RegisterSimpleRequestHandlers<ISimpleRequest, ISimpleRequestHandler<ISimpleRequest>>(out var handlerDescriptors);


            Assert.Single(handlerDescriptors);
            Assert.Equal(mediatrRequestHandlerType, handlerDescriptors[0].ServiceType);
            Assert.Equal(genericRequestHandler, handlerDescriptors[0].ImplementationType);
        }

        [Fact]
        public void Simple_Request_Handler_Delegate_Descriptor_Must_Be_Created()
        {
            var delegateType = typeof(Action<ISimpleRequest, ISimpleRequestHandler<ISimpleRequest>, IServiceProvider>);

            _builder
               .LookIntoAssemblies(assemblies: this.GetType().Assembly)
               .HandleRequests<ISimpleRequest, ISimpleRequestHandler<ISimpleRequest>>(
                by: (request, handler, _) => handler.ShouldHandle(request),
                out var delegateDescriptor);


            Assert.NotNull(delegateDescriptor);
            Assert.Equal(delegateDescriptor.ServiceType, delegateDescriptor.ImplementationType);
            Assert.Equal(delegateType, delegateDescriptor.ServiceType);
        }
    }
}