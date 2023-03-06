namespace MediatR.Latching.Tests
{
    public interface ISimpleRequestHandler<in TRequest> 
        where TRequest : ISimpleRequest
    {
        void ShouldHandle(TRequest request);
    }
}
