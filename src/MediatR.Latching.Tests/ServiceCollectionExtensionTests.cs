using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Latching.Tests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void Related_Service_Descriptors_Must_Be_Added()
        {
            var services = new ServiceCollection();


            var thisAssembly = this.GetType().Assembly;

            services
                .LatchRequestHandlers<ISimpleRequestHandler<ISimpleRequest>, ISimpleRequest>(
                lookInto: this.GetType().Assembly,
                by: (handler, request) => handler.ShouldHandle(request));


            Assert.NotEmpty(services);
            Assert.Equal(3, services.Count);
        }
    }
}
