using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Domain.DDD;
using ProfiBiznes.Shared.Infrastructure;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllValueObjectsShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(ValueObject))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all valueObjects are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllValueObjectsShouldNotContainWritableFieldsTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(ValueObject))
                .And()
                .DoNotInherit(typeof(ValueObjectOf<>))
                .And()
                .AreNotAbstract()
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(w => w.GetProperties()
                    .Any(a => a.CanWrite)
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Value object should not contain writable fields!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void ValueObjectsShouldContainSingleParameterlessConstructorTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var allEntities = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(ValueObject))
                .And()
                .DoNotInherit(typeof(ValueObjectOf<>))
                .GetTypes();

            var missingPrivateConstructorEntities = allEntities.Where(w =>
                    {
                        if (w.IsAbstract)
                            return false;

                        var constructors = w.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                        return !constructors.Any(a => a.IsPrivate &&
                                                      a.GetParameters()
                                                          .Length ==
                                                      0
                        );
                    }
                )
                .ToArray();

            missingPrivateConstructorEntities.Should()
                .BeEmpty("Not all value objects contains private parameterless constructor!: " + missingPrivateConstructorEntities.GetTypesMessage());
        }
    }
}