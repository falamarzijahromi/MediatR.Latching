namespace MediatR.Latching.Tests
{
    public class SimpleRequestHandler : ISimpleRequestHandler<SimpleRequest>
    {
        public void ShouldHandle(SimpleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
