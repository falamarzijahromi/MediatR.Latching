namespace MediatR.Latching
{
    public class Arg<TParameter>
    {
        public static TParameter Parameter { get; } = default;
    }
}
