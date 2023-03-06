namespace MediatR.Latching
{
    public class SimpleRequestWrapper<TRequest> : IRequest<Unit>
    {
        public SimpleRequestWrapper(TRequest request)
        {
            Request = request;
        }

        public TRequest Request { get; }
    }
}
