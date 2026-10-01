using System.Reflection;
using MassTransit.Internals;
using Microsoft.Extensions.DependencyInjection;
using Vero.Shared.ValueObjects;
using static Vero.Shared.Translate.TranslatorEngineSelector;

namespace Vero.Shared.Translate
{
    public sealed class TranslatorEngine
    {
        private readonly IServiceProvider _serviceProvider;

        public TranslatorEngine(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask TranslateAsync(object? obj, int? appLanguageId)
        {
            if (obj is null)
                return;

            if (!appLanguageId.HasValue)
                return;

            await TranslateAsync(obj, AppLanguage.FromId(appLanguageId.Value));
        }

        public async ValueTask TranslateAsync(object? obj, AppLanguage appLanguage)
        {
            if (obj is null)
                return;

            if (appLanguage == AppLanguage.Polish)
                return;

            var data = GetTranslatorPropertyData(obj);
            var translators = GetTranslators(data, _serviceProvider);
            await TranslateAsync(data, translators, appLanguage);
        }

        private static async Task TranslateAsync(IReadOnlyList<TranslatorPropertyData> data, Dictionary<Type, ITranslator> translators, AppLanguage language)
        {
            var dataGroupedByTranslator = data.GroupBy(g => g.Translator.Translator);

            foreach (var propertiesData in dataGroupedByTranslator)
            {
                var translator = translators[propertiesData.Key];
                await translator.InitAsync(
                    propertiesData.Select(s => s.Id)
                        .ToArray(),
                    language
                );

                foreach (var property in propertiesData)
                {
                    TranslateProperty(translator, property, language);
                }
            }
        }

        private static void TranslateProperty(ITranslator translator, TranslatorPropertyData propertyData, AppLanguage appLanguage)
        {
            var newName = translator.GetName(propertyData.Id, appLanguage);
            if (newName is null)
                return;

            propertyData.Name.SetValue(propertyData.Parent, newName);
        }

        private static Dictionary<Type, ITranslator> GetTranslators(IReadOnlyList<TranslatorPropertyData> data, IServiceProvider serviceProvider) => data
            .Select(s => s.Translator.Translator)
            .Distinct()
            .ToDictionary(k => k, v => CreateITranslator(v, serviceProvider));

        private static ITranslator CreateITranslator(Type translator, IServiceProvider serviceProvider)
        {
            if (!translator.HasInterface(typeof(ITranslator)))
                throw new Exception("Translator nie implementuje ITranslator");

            var constructors = translator.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault();

            if (constructors is null)
                throw new InvalidOperationException("There is no public constructor in DBMigrator");

            var constructorParametersIstance = constructors.GetParameters()
                .Select(s => serviceProvider.GetRequiredService(s.ParameterType))
                .ToArray();

            var iTranslatorInstance = Activator.CreateInstance(translator, constructorParametersIstance);
            if (iTranslatorInstance is null)
                throw new Exception("Failed to create DBMigrator instance");

            return (iTranslatorInstance as ITranslator)!;
        }
    }
}
