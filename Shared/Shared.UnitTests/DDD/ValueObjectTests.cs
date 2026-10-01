using FluentAssertions;
using Vero.Shared.DDD;
using Vero.Shared.ValueObjects;
using Xunit;

namespace Shared.UnitTests.DDD
{
    
    public sealed class ValueObjectTests
    {
        private sealed record TestValueObject : ValueObjectOf<int>
        {
            public TestValueObject(int Value) : base(Value)
            {
                if (Value < 0)
                    throw new Exception();
            }
        }

        private sealed record Test2ValueObject : ValueObjectOf<int>
        {
            public Test2ValueObject(int aaa) : base(aaa)
            {
                if (Value < 0)
                    throw new Exception();
            }
        }

        private sealed record Test3ValueObject : ValueObjectOf<int>
        {
            public Test3ValueObject(int aaa, int bbb) : base(aaa)
            {
                Bbb = bbb;

                if (Value < 0 || Bbb < 0)
                    throw new Exception();
            }

            public int Bbb { get; }
        }

        [Fact]
        public void Validation_throw_exception()
        {
            var act = () => new TestValueObject(-10);
            act.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void Validation_throw_exception_with_custom_constructor()
        {
            var act = () => new Test2ValueObject(-10);
            act.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void Implicit_conversion_equal_to_value()
        {
            var initValue = 13;
            var vo = new Test2ValueObject(initValue);
            int voValue = vo;

            initValue.Should()
                .Be(vo.Value);

            initValue.Should()
                .Be(voValue);
        }

        [Fact]
        public void Value_object_should_be_equals()
        {
            var value = 152663;

            var valO1 = new TestValueObject(value);
            var valO2 = new TestValueObject(value);

            valO1.Value.Should()
                .Be(value);

            valO1.Should()
                .Be(valO2);

            valO1.Value.Should()
                .Be(valO2.Value);

            valO1.GetHashCode()
                .Should()
                .Be(valO2.GetHashCode());

            valO1.ToString()
                .Should()
                .Be(valO2.ToString());
        }

        [Fact]
        public void Validation_throw_exception_with_custom_property()
        {
            var act = () => new Test3ValueObject(10, -10);
            act.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ToString_give_value_string()
        {
            var val = new TestValueObject(10);

            val.ToString()
                .Should()
                .Be("10");
        }

        [Fact]
        public void Value_object_with_custom_properties_should_be_equals()
        {
            var value1 = 152663;
            var value2 = 34556;

            var valO1 = new Test3ValueObject(value1, value2);
            var valO2 = new Test3ValueObject(value1, value2);

            valO1.Should()
                .Be(valO2);

            valO1.Value.Should()
                .Be(valO2.Value);

            valO1.GetHashCode()
                .Should()
                .Be(valO2.GetHashCode());

            valO1.ToString()
                .Should()
                .Be(valO2.ToString());
        }

        [Fact]
        public void PostCode_IsParentOf_returns_true_when_postcode_is_child()
        {
            var parent = new CleanPostCode("34");
            var child = new CleanPostCode("34300");

            parent.IsParentOf(child)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void PostCode_IsParentOf_returns_false_when_postcode_is_not_a_child()
        {
            var parent = new CleanPostCode("34300");
            var child = new CleanPostCode("34");

            parent.IsParentOf(child)
                .Should()
                .BeFalse();
        }
    }
}