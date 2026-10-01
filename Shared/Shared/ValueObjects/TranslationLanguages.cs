using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record TranslationLanguages : ValueObject
    {
        public TranslationLanguages(ForeignLanguage english, ForeignLanguage german)
        {
            English = english;
            German = german;
        }

        public ForeignLanguage English { get; }

        public ForeignLanguage German { get; }

        public bool Equals(TranslationLanguages? other)
        {
            if (other == null || GetType() != other.GetType())
                return false;

            return English.Equals(other.English) && German.Equals(other.German);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public sealed record ForeignLanguage : ValueObjectOf<string>
    {
        public ForeignLanguage(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ForeignLanguageEmptyException();

            Value = value.Trim();
        }
    }
}