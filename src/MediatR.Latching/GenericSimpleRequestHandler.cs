using System;

namespace MediatR.Latching
{
    public class GenericSimpleRequestHandler<TRequestHandler, TRequestHandlerBase, TRequestWrapper, TRequest, TRequestBase> : RequestHandler<TRequestWrapper>
        where TRequestWrapper : SimpleRequestWrapper<TRequest>
        where TRequestHandlerBase : class
        where TRequestBase: class
        where TRequestHandler : class
        where TRequest : TRequestBase
    {
        private readonly TRequestHandler requestHandler;
        private readonly Action<TRequestHandler, TRequestBase, IServiceProvider> handlerDelegate;
        private readonly IServiceProvider serviceProvider;

        public GenericSimpleRequestHandler(
            Action<TRequestHandler, TRequestBase, IServiceProvider> handlerDelegate,
            IServiceProvider serviceProvider,
            TRequestHandler requestHandler)
        {
            this.requestHandler = requestHandler;
            this.handlerDelegate = handlerDelegate;
            this.serviceProvider = serviceProvider;
        }

        protected override void HandleCore(TRequestWrapper message)
        {
            handlerDelegate(requestHandler, message.Request, serviceProvider);
        }
    }
}
