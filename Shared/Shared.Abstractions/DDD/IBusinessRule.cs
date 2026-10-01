namespace Vero.Shared.Abstractions.DDD
{
    public interface IBusinessRule
    {
        string Message { get; }

        string Code { get; }

        bool IsBroken();
    }
}