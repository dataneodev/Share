using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NetArchTest.Rules;
using Vero.Shared.DDD;
using Xunit;

namespace Shared.ArchTests.Domain
{
    public sealed class ValueObjectTests
    {
        //[Fact]
        //public void ValueObject_Should_Have_PrivateSetter()
        //{
        //    var valueObjectsTypes = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
        //        .That()
        //        .AreClasses()
        //        .And()
        //        .Inherit(typeof(ValueObject))
        //        .GetTypes();

        //    var valueObjectWithPublicSetter = valueObjectsTypes.Where(HasPublicSetter)
        //        .ToArray();

        //    valueObjectWithPublicSetter.Should()
        //        .BeEmpty("ValueObject can't have public setters!: " + valueObjectWithPublicSetter.GetTypesMessage());
        //}

        public static bool HasPublicSetter(Type type)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (HasPublicSetter(property))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasPublicSetter(PropertyInfo property)
        {
            if (property.CanWrite && property.SetMethod != null && property.SetMethod.IsPublic)
            {
                var setMethodReturnParameterModifiers = property.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                var initOnly = setMethodReturnParameterModifiers.Contains(typeof(IsExternalInit));
                return !initOnly;
            }

            return false;
        }

        [Fact]
        public void AllValueObjectsShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(ValueObject))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all valueObjects are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}