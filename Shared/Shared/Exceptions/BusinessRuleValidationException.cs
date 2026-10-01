using Vero.Shared.Abstractions.DDD;

namespace Vero.Shared.Exceptions
{
    public sealed class BusinessRuleValidationException : AppException
    {
        public BusinessRuleValidationException(IBusinessRule rule) : base(rule.Message)
        {
            Rule = rule;
            Details = rule.Message;
            Code = rule.Code;
        }

        public IBusinessRule Rule { get; }

        public string Code { get; }

        public string Details { get; }

        public override string ToString() => $"{Rule.GetType().FullName}: {Rule.Message}";
    }
}