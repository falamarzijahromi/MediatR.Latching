namespace MediatR.Latching.OpTest
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void ThisMustHandleCommand(TCommand command);
    }

    public class CommandSample : ICommand
    {
        public int Test { get; set; } = 3;
    }

    public class CommandSampleDoer : ICommandHandler<CommandSample>
    {
        public void ThisMustHandleCommand(CommandSample command)
        {
        }
    }
}
