using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Vero.Shared.ModelBinders
{
    /// <summary>
    ///     This class implements a binder for handling a JSON and file multi-part form data request.
    /// </summary>
    public sealed class JsonWithFilesFormDataModelBinder : IModelBinder
    {
        private readonly FormFileModelBinder formFileModelBinder;
        private readonly JsonSerializerSettings options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonWithFilesFormDataModelBinder" /> class.
        /// </summary>
        /// <param name="jsonSettings">Contains the Json serializer settings.</param>
        /// <param name="loggerFactory">Contains a logger factory instance.</param>
        public JsonWithFilesFormDataModelBinder(ILoggerFactory loggerFactory)
        {
            options = new JsonSerializerSettings();
            formFileModelBinder = new FormFileModelBinder(loggerFactory);
        }

        /// <summary>
        ///     This method is used to bind models included in multi-part form requests.
        /// </summary>
        /// <param name="bindingContext">Contains the binding context.</param>
        /// <returns>Returns a task result.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the model context is not defined.</exception>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            // check for form JSON multi-part by field name
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            // if the JSON part was found...
            if (valueResult != ValueProviderResult.None)
            {
                // get the raw JSON data.
                var rawValue = valueResult.FirstValue;

                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    // Deserialize the JSON
                    var model = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType, options);

                    // get IFormFile properties in the bound model.
                    var formFileProperties = bindingContext.ModelMetadata.Properties.Where(prop => prop.ModelType == typeof(IFormFile))
                        .ToList();

                    // for each property, bind each of the IFormFile properties from the other multi-request parts
                    foreach (var property in formFileProperties)
                    {
                        var fieldName = property.BinderModelName ?? property.PropertyName;
                        var modelName = fieldName ?? string.Empty;

                        if (bindingContext.Model != null)
                        {
#pragma warning disable CS8602// Dereference of a possibly null reference.
                            var propertyModel = property.PropertyGetter(bindingContext.Model);
#pragma warning restore CS8602// Dereference of a possibly null reference.
                            ModelBindingResult propertyResult;

                            if (propertyModel != null)
                            {
#pragma warning disable CS8604// Possible null reference argument.
                                using (bindingContext.EnterNestedScope(property, fieldName, modelName, propertyModel))
                                {
                                    await formFileModelBinder.BindModelAsync(bindingContext);
                                    propertyResult = bindingContext.Result;
                                }
#pragma warning restore CS8604// Possible null reference argument.

                                // if the IFormFile was sucessfully bound, assign it to the corresponding property of the model
                                if (propertyResult.IsModelSet)
                                {
#pragma warning disable CS8602// Dereference of a possibly null reference.
#pragma warning disable CS8604// Possible null reference argument.
                                    property.PropertySetter(model, propertyResult.Model);
#pragma warning restore CS8604// Possible null reference argument.
#pragma warning restore CS8602// Dereference of a possibly null reference.
                                }
                                else if (property.IsBindingRequired)
                                {
                                    var message = property.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(fieldName);
                                    bindingContext.ModelState.TryAddModelError(modelName, message);
                                }
                            }
                            else
                            {
                                bindingContext.ModelState.TryAddModelError(modelName, $"Missing {nameof(propertyModel)}");
                            }
                        }
                        else
                        {
                            bindingContext.ModelState.TryAddModelError(modelName, $"Missing {nameof(bindingContext.Model)}");
                        }
                    }

                    // Set the successfully constructed model as the result of the model binding
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
                else
                {
                    // The JSON field binding was not found
                    var message = bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(bindingContext.FieldName);
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);
                }
            }
            else
            {
                // The JSON field binding was not found
                var message = bindingContext.ModelMetadata.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(bindingContext.FieldName);
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);
            }
        }
    }
}