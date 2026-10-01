using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vero.Shared.ModelBinders
{
    public sealed class DateTimeOffsetModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.UnderlyingOrModelType == typeof(DateTimeOffset))
            {
                return new DateTimeOffsetModelBinder();
            }

            return null;
        }
    }
}