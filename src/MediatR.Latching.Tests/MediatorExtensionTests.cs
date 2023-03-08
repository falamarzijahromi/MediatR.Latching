using NSubstitute;

namespace MediatR.Latching.Tests
{
    public class MediatorExtensionTests
    {
        [Fact]
        public void Mediator_Should_Send_Request_Wrapper()
        {
            var mediatorMock = Substitute.For<IMediator>();

            mediatorMock.Send(Arg.Do<IRequest<Unit>>(AssertCall));

            mediatorMock.SendRequest(new SimpleRequest());

            mediatorMock.Received(1);
        }

        private static void AssertCall(IRequest<Unit> wrappedRequest)
        {
            Assert.NotNull(wrappedRequest);

            Assert.True(wrappedRequest is SimpleRequestWrapper<SimpleRequest>);
        }
    }
}
