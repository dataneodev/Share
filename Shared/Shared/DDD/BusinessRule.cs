using Vero.Shared.Abstractions.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.DDD
{
    public abstract class BusinessRule : IBusinessRule
    {
        public abstract string Message { get; }

        public string Code => GetType()
            .Name.ToSnakeCase();

        public abstract bool IsBroken();
    }
}