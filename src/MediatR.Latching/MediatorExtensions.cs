using System;
using System.Threading.Tasks;

namespace MediatR.Latching
{
    public static class MediatorExtensions
    {
        public static Task<Unit> SendRequest<TRequest>(this IMediator mediator, TRequest request)
        {
            var simpleRequestWrapperType = typeof(SimpleRequestWrapper<>).MakeGenericType(typeof(TRequest));

            var simpleRequest = Activator.CreateInstance(simpleRequestWrapperType, request) as IRequest<Unit>;

            return mediator.Send(simpleRequest);
        }
    }
}
