using System.Linq.Expressions;
using System.Reflection;

namespace Vero.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<KeyValuePair<Type, Type>> GetGenericWithGenericArgumentType(this Type genericType, Assembly assembly) => assembly.GetTypes()
            .Where(
                t => t.IsClass &&
                     t.BaseType != null &&
                     t.BaseType.IsConstructedGenericType &&
                     t.BaseType.GenericTypeArguments.Any() &&
                     t.BaseType.Name == genericType.Name
            )
            .Select(t => new KeyValuePair<Type, Type>(t, t.BaseType!.GenericTypeArguments[0]));

        public static IEnumerable<Type> GetInheritingTypes(this Type baseType, Assembly assembly) => assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract && t.BaseType == baseType);

        public static IEnumerable<Type> GetInheritingTypes<T>(this Assembly assembly)
            where T : class => assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract && IsBaseType(t.BaseType, typeof(T)));

        public static IEnumerable<Type> GetTypesImplementingInterface<T>(this Assembly assembly) => assembly.GetTypes()
            .Where(
                t => t.GetInterfaces()
                    .Any(i => i == typeof(T))
            );
        public static IEnumerable<Type> GetTypesImplementingInterface<T>(this List<Assembly> assemblies)
        {
            return assemblies.SelectMany(assembly => 
                assembly.GetTypes()
                    .Where(type => type.GetInterfaces()
                        .Any(i => i == typeof(T)))
            );
        }
        private static bool IsBaseType(Type? baseType, Type find)
        {
            if (baseType is null)
                return false;

            if (baseType == find)
                return true;

            return IsBaseType(baseType.BaseType, find);
        }
    }

    public static class Property
    {
        public static string[] GetFullNames<T>(params Expression<Func<T, object>>[] expressions) => expressions
            .SelectMany(expression => GetPropertyFullNames(expression.Body))
            .ToArray();

        private static IEnumerable<string> GetPropertyFullNames(Expression expression)
        {
            var stack = new Stack<string>();
            var memberExpression = expression as MemberExpression;

            while (memberExpression != null)
            {
                stack.Push(memberExpression.Member.Name);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            if (stack.Count > 0)
            {
                yield return string.Join(".", stack);
            }
            else
            {
                throw new ArgumentException("The expression must be a MemberExpression or a BinaryExpression.", nameof(expression));
            }
        }
    }
}