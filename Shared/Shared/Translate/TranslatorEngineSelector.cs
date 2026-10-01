using System.Collections;
using System.Reflection;

namespace Vero.Shared.Translate
{
    internal static class TranslatorEngineSelector
    {
        public static IReadOnlyList<TranslatorPropertyData> GetTranslatorPropertyData(object obj)
        {
            var translatorProperiesData = new List<TranslatorPropertyData>();

            foreach (var prop in obj.GetType()
                         .GetProperties())
            {
                ProcessProperty(prop, obj, translatorProperiesData);
            }

            return translatorProperiesData;
        }

        private static void ProcessProperty(PropertyInfo prop, object obj, List<TranslatorPropertyData> result)
        {
            if (obj is null || prop is null)
                return;

            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
            {
                var collection = (IEnumerable?)prop.GetValue(obj, null);
                if (collection is null)
                    return;

                var translator = GetTranslator(prop);

                foreach (var collectionItem in collection)
                {
                    if (translator is not null)
                    {
                        var translateObjectData = TranslateObject(collectionItem, translator);
                        if (translateObjectData != null)
                            result.Add(translateObjectData);
                    }
                    else
                    {
                        foreach (var collectionItemProp in collectionItem.GetType()
                                     .GetProperties())
                        {
                            ProcessProperty(collectionItemProp, collectionItem, result);
                        }
                    }
                }

                return;
            }

            if (!prop.PropertyType.IsClass || prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                return;

            var translateData = TranslateProperty(prop, obj);
            if (translateData != null)
                result.Add(translateData);

            var property = prop.GetValue(obj);

            if (property is null)
                return;

            var propertyType = property.GetType();

            if (!propertyType.IsClass || propertyType.IsPrimitive)
                return;

            foreach (var innerProp in propertyType.GetProperties())
            {
                ProcessProperty(innerProp, property, result);
            }
        }

        private static TranslatorPropertyData? TranslateProperty(PropertyInfo prop, object obj)
        {
            var translator = GetTranslator(prop);
            var property = prop.GetValue(obj);

            return TranslateObject(property, translator);
        }

        private static TranslatorAttribute? GetTranslator(PropertyInfo prop) => prop.GetCustomAttributes(false)
            .FirstOrDefault(f => f is TranslatorAttribute) as TranslatorAttribute;

        private static TranslatorPropertyData? TranslateObject(object? property, TranslatorAttribute? translator)
        {
            if (property is null || translator is null)
                return null;

            var type = property.GetType();

            var idProp = type.GetProperty("Id");

            if (idProp is null)
                throw new Exception("Obiekt do tłumaczenia nie zawiera właściwiości Id");

            var nameProp = type.GetProperty(translator.Name);

            if (nameProp is null)
                throw new Exception("Obiekt do tłumaczenia nie zawiera właściwiości Name");

            var prop = idProp.GetValue(property);
            if (prop is null)
                throw new Exception("Obiekt do tłumaczenia nie zawiera właściwiości Id");

            var id = (int)prop;

            return new TranslatorPropertyData(translator, id, nameProp, property);
        }

        public sealed record TranslatorPropertyData(TranslatorAttribute Translator, int Id, PropertyInfo Name, object Parent);
    }
}