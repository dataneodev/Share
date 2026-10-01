using System.Reflection;
using AutoMapper;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.AutoMapper
{
    public static class ValueObjectOfAssembly
    {
        public static void AddValueObjectOfMap(this Profile profile, params Assembly[] assemblies)
        {
            var valueObjectOfTypes = assemblies.SelectMany(assembly => GetValueObjectOfTypes(assembly))
                .ToArray();

            AddTypesToProfile(profile, valueObjectOfTypes);
        }

        public static void CreateValueObjectMap<TValueObjectOf, TPrimitive>(Profile profile)
            where TValueObjectOf : ValueObjectOf<TPrimitive> => profile.CreateMap<TValueObjectOf, TPrimitive>()
            .ConstructUsing(source => source.Value!);

        private static void AddTypesToProfile(Profile profile, IReadOnlyList<KeyValuePair<Type, Type>> mapTypes)
        {
            var exeMethod = typeof(ValueObjectOfAssembly).GetMethod("CreateValueObjectMap");
            var arguments = new object[] { profile };

            foreach (var mapType in mapTypes)
            {
                CreateTypeMap(mapType.Key, mapType.Value, exeMethod!, arguments);
            }
        }

        private static IEnumerable<KeyValuePair<Type, Type>> GetValueObjectOfTypes(Assembly assembly) =>
            typeof(ValueObjectOf<>).GetGenericWithGenericArgumentType(assembly);

        private static void CreateTypeMap(Type valueObject, Type primitive, MethodInfo exeMethod, object[] arguments)
        {
            var registerMethod = exeMethod.MakeGenericMethod(valueObject, primitive);
            registerMethod.Invoke(null, arguments);
        }
    }
}