using System.Reflection;
using AutoMapper;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.AutoMapper
{
    public static class EnumerationAssembly
    {
        public static void AddEnumerationMap(this Profile profile, params Assembly[] assemblies)
        {
            var enumeraionTypes = assemblies.SelectMany(GetEnumerationTypes)
                .ToArray();

            AddMappedTypes(enumeraionTypes, profile);
        }

        public static void CreateEnumerationtMap<TEnumeration, T>(Profile profile)
            where TEnumeration : Enumeration<T>
            where T : Enumeration<T> => profile.CreateMap<TEnumeration, int>()
            .ConstructUsing(source => source.Id);

        private static IEnumerable<KeyValuePair<Type, Type>> GetEnumerationTypes(Assembly assembly) =>
            typeof(Enumeration<>).GetGenericWithGenericArgumentType(assembly);

        private static void AddMappedTypes(IReadOnlyList<KeyValuePair<Type, Type>> mapTypes, Profile profile)
        {
            var exeMethod = typeof(EnumerationAssembly).GetMethod("CreateEnumerationtMap");
            var argument = new object[] { profile };

            foreach (var mapType in mapTypes)
            {
                CreateTypeMap(mapType.Key, mapType.Value, exeMethod!, argument);
            }
        }

        private static void CreateTypeMap(Type valueObject, Type primitive, MethodInfo exeMethod, object[] profile)
        {
            var registerMethod = exeMethod.MakeGenericMethod(valueObject, primitive);
            registerMethod.Invoke(null, profile);
        }
    }
}