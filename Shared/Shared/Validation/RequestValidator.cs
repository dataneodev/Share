using FluentValidation;
using FluentValidation.Results;
using Vero.Shared.Security;

namespace Vero.Shared.Validation
{
    public abstract class RequestValidator<T> : AbstractValidator<T>
    {
        private readonly IContextAccessor _accessor;
        private readonly Permission[] _allowed;

        public RequestValidator(IContextAccessor accessor, params Permission[] allowed)
        {
            _accessor = accessor;
            _allowed = allowed;
        }

        protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        {
            _accessor.CheckPermission(_allowed);
            return base.PreValidate(context, result);
        }
    }
}