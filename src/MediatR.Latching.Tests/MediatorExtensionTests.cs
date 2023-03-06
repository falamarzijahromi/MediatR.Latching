using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using MediatR.Latching;

namespace MediatR.Latching.Tests
{
    public class MediatorExtensionTests
    {
        [Fact]
        public void Mediator_Should_Send_Request_Wrapper()
        {
            var mediatorMock = Substitute.For<IMediator>();

            mediatorMock.Send(Arg.Do<IRequest>(AssertCall));

            mediatorMock.SendRequest(new SimpleRequest());

            mediatorMock.Received(1);
        }

        private static void AssertCall(IRequest wrappedRequest)
        {
            Assert.NotNull(wrappedRequest);

            Assert.True(wrappedRequest is SimpleRequestWrapper<SimpleRequest>);
        }
    }
}
