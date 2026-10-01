namespace Vero.Shared.MessageBroker
{
    public sealed class MessageBrokerOptions
    {
        public ushort Port { get; init; }

        public string HostName { get; init; }

        public string UserName { get; init; }

        public string Password { get; init; }
    }
}