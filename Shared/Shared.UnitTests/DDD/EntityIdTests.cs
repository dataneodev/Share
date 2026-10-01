using FluentAssertions;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;
using Xunit;


namespace Shared.UnitTests.DDD
{
    public sealed class EntityIdTests
    {
        private sealed record TestEntityId(int Value) : EntityId(Value);
        
        [Fact]
        public void EntityId_with_the_same_value_should_be_equal()
        {
            const int idValue = 253658;

            var id1 = new TestEntityId(idValue);
            var id2 = new TestEntityId(idValue);

            id1.Should()
                .Be(id2);

            id1.Value.Should()
                .Be(id2.Value);

            id1.Value.Should()
                .Be(idValue);
        }

        [Fact]
        public void EntityId_with_different_value_should_not_be_equal()
        {
            const int idValue1 = 253658;
            const int idValue2 = 36524;

            var id1 = new TestEntityId(idValue1);
            var id2 = new TestEntityId(idValue2);

            id1.Should()
                .NotBe(id2);

            id1.Value.Should()
                .NotBe(id2.Value);
        }

        [Fact]
        public void EntityId_should_throw_on_negative_value()
        {
            const int idValue = -3652;
            var testAction = () => new TestEntityId(idValue);
            testAction.Should()
                .Throw<InvalidEntityIdValueException>();
        }

        [Fact]
        public void EntityId_should_throw_on_empty_value()
        {
            const int idValue = 0;
            var testAction = () => new TestEntityId(idValue);
            testAction.Should()
                .Throw<InvalidEntityIdValueException>();
        }

        [Fact]
        public void Implicit_conversion_should_give_id()
        {
            const int idValue1 = 253658;
            
            var id1 = new TestEntityId(idValue1);
            int idFromImplicitConversion = id1;
            idFromImplicitConversion.Should()
                .Be(idValue1);
        }

        [Fact]
        public void EntityId_with_the_same_value_should_have_the_same_hashcode()
        {
            const int idValue = 253658;

            var id1 = new TestEntityId(idValue);
            var id2 = new TestEntityId(idValue);

            id1.GetHashCode()
                .Should()
                .Be(id2.GetHashCode());
        }

        [Fact]
        public void HashSet_with_entity_should_contains_correct()
        {
            const int idValue = 253658;
            var id1 = new TestEntityId(idValue);
            var id2 = new TestEntityId(idValue);

            var hashset = new HashSet<TestEntityId>();
            hashset.Add(id1);

            hashset.Contains(id1)
                .Should()
                .BeTrue();

            hashset.Contains(id2)
                .Should()
                .BeTrue();
        }
        
    }
}