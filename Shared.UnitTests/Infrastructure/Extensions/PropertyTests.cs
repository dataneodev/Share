using System.Linq.Expressions;
using FluentAssertions;
using ProfiBiznes.Shared.Application.Extensions;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Extensions
{
    public sealed class PropertyTests
    {
        [Fact]
        public void GetFullNames_NestedClass_ReturnsCorrectlyNestedResults()
        {
            // Arrange
            var expressions = new Expression<Func<BaseClass, object>>[] { x => x.Name, x => x.Nested1.Name1, x => x.Nested1.Nested2.Name2 };

            // Act
            var result = Property.GetFullNames(expressions);

            // Assert
            result.Should()
                .Contain("Name");

            result.Should()
                .Contain("Nested1.Name1");

            result.Should()
                .Contain("Nested1.Nested2.Name2");
        }

        internal record BaseClass(string Name, BaseClass.BaseClass_Nested1 Nested1)
        {
            internal record BaseClass_Nested1(string Name1, BaseClass_Nested1.BaseClass_Nested2 Nested2)
            {
                internal record BaseClass_Nested2(string Name2)
                {
                }
            }
        }
    }
}