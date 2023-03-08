using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Latching
{
    public class GenericSimpleRequestHandler<TRequestHandler, TRequestWrapper, TRequest> : IRequestHandler<TRequestWrapper, Unit>
        where TRequestWrapper : SimpleRequestWrapper<TRequest>
    {
        private readonly TRequestHandler requestHandler;
        private readonly Action<TRequestHandler, TRequest> handlerDelegate;

        public GenericSimpleRequestHandler(
            Action<TRequestHandler, TRequest> handlerDelegate,
            TRequestHandler requestHandler)
        {
            this.handlerDelegate = handlerDelegate;
            this.requestHandler = requestHandler;
        }

        public Task<Unit> Handle(TRequestWrapper requestWrapper, CancellationToken cancellationToken)
        {
            handlerDelegate(requestHandler, requestWrapper.Request);

            return Task.FromResult(Unit.Value);
        }
    }
}
