using System.Linq.Expressions;

namespace Vero.Shared.MartenExtensions
{
    public static class Property
    {
        public static string[] GetFullNames<T>(params Expression<Func<T, object>>[] expressions) => expressions
            .SelectMany(expression => GetPropertyFullNames(expression.Body))
            .ToArray();

        private static IEnumerable<string> GetPropertyFullNames(Expression expression)
        {
            var stack = new Stack<string>();

            var memberExpression = expression as MemberExpression;

            if (memberExpression is null && expression is UnaryExpression unaryExpression)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

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