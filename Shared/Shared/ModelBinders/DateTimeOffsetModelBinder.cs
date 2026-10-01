using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vero.Shared.ModelBinders
{
    public sealed class DateTimeOffsetModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                // no entry found for some of the two needed input fields
                //return without setting result, that is, return a failure
                return Task.CompletedTask;
            }

            //store value retrieved for Date+Time in model state
            //this way, in case of format errors user may
            //correct input. No need to store also time zone offset,
            //since it cannot be edited by the user
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;

                object? model;
                if (string.IsNullOrWhiteSpace(value))
                {
                    //empty fields, means null model
                    model = null;
                }
                else
                {
                    model = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture)
                        .ToUniversalTime();
                }

                //if model is null and type is not nullable
                //return a required field error
                if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
                {
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(valueProviderResult.ToString())
                    );

                    return Task.CompletedTask;
                }

                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                //in case parsers throw a FormatException
                //add error to the model state.

                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);


                return Task.CompletedTask;
            }
        }
    }
}