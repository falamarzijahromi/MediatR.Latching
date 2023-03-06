using System;

namespace MediatR.Latching
{
    public static class MediatorExtensions
    {
        public static void SendRequest<TRequest>(this IMediator mediator, TRequest request)
        {
            var simpleRequestWrapperType = typeof(SimpleRequestWrapper<>).MakeGenericType(typeof(TRequest));

            var simpleRequest = Activator.CreateInstance(simpleRequestWrapperType, request) as IRequest<Unit>;

            mediator.Send(simpleRequest);
        }
    }
}
