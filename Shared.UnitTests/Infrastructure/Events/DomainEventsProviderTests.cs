using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ProfiBiznes.Shared.Domain.DDD;
using ProfiBiznes.Shared.Infrastructure.Database.EF;
using ProfiBiznes.Shared.Infrastructure.Events;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Events
{
    public sealed class DomainEventsProviderTests
    {
        [Fact]
        public async Task PopAllDomainEvents_TrackedEntitiesWithDomainEvents_PopsAllAvailableDomainEvents()
        {
            // Arrange
            await using var dbContext = new DummyDbContext();
            await dbContext.Database.EnsureCreatedAsync();
            var entity = new Entity1();
            dbContext.Entity1S.Add(entity);

            var dbContextFactory = Substitute.For<IDbContextFactory>();
            dbContextFactory.Create()
                .Returns(dbContext);

            dbContextFactory.Current.Returns(dbContext);

            var domainEventsProvider = new DomainEventsProvider(dbContextFactory);

            // Act
            var domainEvents = domainEventsProvider.PopAllDomainEvents();

            // Assert
            domainEvents.Should()
                .ContainSingle(e => e is DummyDomainEvent);

            entity.DomainEvents
                .Should()
                .BeEmpty();
        }

        private sealed class DummyDbContext : DbContext
        {
            public DbSet<Entity1> Entity1S { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase("Test");

            protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Entity1>(b => { b.Ignore(p => p.DomainEvents); });
        }

        private sealed record DummyDomainEvent : DomainEvent
        {
        }

        private sealed class Entity1 : Entity
        {
            public Entity1() => AddDomainEvent(new DummyDomainEvent());

            public int Id { get; set; }
        }
    }
}