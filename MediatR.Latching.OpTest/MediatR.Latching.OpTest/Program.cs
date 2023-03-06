using MediatR;
using MediatR.Latching;

namespace MediatR.Latching.OpTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services
                .AddMediatR(typeof(Program).Assembly)
                .LatchRequestHandlers<ICommandHandler<ICommand>, ICommand>(
                    lookInto: typeof(Program).Assembly,
                    by: (command, handler, _) => handler.ThisMustHandleCommand(command));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}