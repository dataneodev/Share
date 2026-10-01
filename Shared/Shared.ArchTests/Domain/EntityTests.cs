using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Vero.Shared.DDD;
using Xunit;

namespace Shared.ArchTests.Domain
{
    public sealed class EntityTests
    {
        [Fact]
        public void EntityMustContainsPrivateParameterlessConstructorTest()
        {
            var allEntities = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(Entity<>))
                .Or()
                .Inherit(typeof(Entity))
                .GetTypes();

            var missingPrivateConstructorEntities = allEntities.Where(
                    w =>
                    {
                        if (w.IsAbstract)
                            return false;

                        var constructors = w.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                        return !constructors.Any(
                            a => a.IsPrivate &&
                                 a.GetParameters()
                                     .Length ==
                                 0
                        );
                    }
                )
                .ToArray();

            missingPrivateConstructorEntities.Should()
                .BeEmpty("Not all entites contains private parameterless constructor!: " + missingPrivateConstructorEntities.GetTypesMessage());
        }

        //[Fact]
        //public void EntityShouldNotContainsPublicConstructorTest()
        //{
        //    var allEntities = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
        //        .That()
        //        .Inherit(typeof(Entity<>))
        //        .Or()
        //        .Inherit(typeof(Entity))
        //        .GetTypes();

        //    var missingPrivateConstructorEntities = allEntities.Where(
        //            w => w.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        //                .Any()
        //        )
        //        .ToArray();

        //    missingPrivateConstructorEntities.Should()
        //        .BeEmpty("Entity should not contains public constructor!: " + missingPrivateConstructorEntities.GetTypesMessage());
        //}

        [Fact]
        public void AllEntitiesShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
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

            result.IsSuccessful.Should()
                .BeTrue("Not all entites are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllEntitiesShouldResideInDomainTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(Entity<>))
                .Or()
                .Inherit(typeof(Entity))
                .Should()
                .ResideInNamespaceContaining(".Domain.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Entity should reside only in domain layer!: " + result.GetFailingTypesMessage());
        }
    }
}