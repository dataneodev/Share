namespace Vero.Shared.MessageBroker
{
    public interface IRequestBus
    {
    }

    public interface IRequestBus<TResponseValue> : IRequestBus
    {
    }
}