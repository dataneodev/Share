using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Shared.UnitTests.Swagger
{
    public sealed class ConstTypeExtractorTests
    {
        private static string GetFieldValue(Type type, string constName)
        {
            const string defaultValue = "0";
            return type?.GetField(constName, BindingFlags.NonPublic | BindingFlags.Static)
                       ?.GetValue(null)
                       ?.ToString() ??
                   defaultValue;
        }

        [Fact]
        public void Extract_const_success() => GetFieldValue(typeof(TestClass2), "B")
            .Should()
            .Be("7");
    }

    internal sealed class TestClass2 : TestClass
    {
        private new static int B = 7;
    }

    internal class TestClass
    {
        protected static int B;

        public int A => B;
    }
}