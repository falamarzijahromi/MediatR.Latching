using System.Reflection;

namespace MediatR.Latching
{
    public class RequestHandlerTypeFounder
    {
        private Assembly[] assemblies;

        public RequestHandlerTypeFounder(Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }
    }
}