using System.Net.Mail;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record EmailAddress : ValueObjectOf<string>
    {
        public EmailAddress(string value) : base(value)
        {
            if (!MailAddress.TryCreate(value, out var email))
                throw new IncorrectEmailAddressFormatException(value);

            Value = email.Address;
        }
    }

    internal sealed class IncorrectEmailAddressFormatException : AppException
    {
        public IncorrectEmailAddressFormatException(string value) : base($"Niepoprawny format adresu email: {value}!")
        {
        }
    }
}