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
        public void AllEntitiesShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Entity<>))
                .And()
                .AreNotAbstract()
                .Or()
                .Inherit(typeof(Entity))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all entities are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllEntitiesShouldResideInDomainTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Entity<>))
                .Or()
                .Inherit(typeof(Entity))
                .Should()
                .ResideInNamespaceContaining(".Domain.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Entity should reside only in domain layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void EntityMustContainsPrivateSingleParameterConstructorTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var allEntities = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Entity<>))
                .Or()
                .Inherit(typeof(Entity))
                .GetTypes();

            var missingPrivateConstructorEntities = allEntities.Where(w =>
                    {
                        if (w.IsAbstract)
                            return false;

                        var properties = w.GetProperties()
                            .Where(x => x.Name != "Version" && x.Name != "DomainEvents" && x.PropertyType.BaseType != typeof(EntityId));

                        if (!properties.Any())
                            return false;

                        var constructors = w.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                        return !constructors.Any(a => a.IsPrivate &&
                                                      a.GetParameters()
                                                          .Length ==
                                                      1
                        );
                    }
                )
                .ToArray();

            missingPrivateConstructorEntities.Should()
                .BeEmpty("Not all entities contains private single parameter constructor!: " + missingPrivateConstructorEntities.GetTypesMessage());
        }
    }
}