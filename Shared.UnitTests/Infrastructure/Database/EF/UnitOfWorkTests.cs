using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ProfiBiznes.Shared.Domain.DDD;
using ProfiBiznes.Shared.Infrastructure.Database.EF;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Database.EF
{
    public sealed class UnitOfWorkTests
    {
        [Fact]
        public async Task Save_ModifiedEntities_IncrementsVersion()
        {
            // Arrange
            await using var context = new DummyDbContext();
            await context.Database.EnsureCreatedAsync();

            var entity = new Entity1();
            context.Entity1S.Add(entity);
            await context.SaveChangesAsync();

            var dbContextFactory = Substitute.For<IDbContextFactory>();
            dbContextFactory.Create()
                .Returns(context);

            dbContextFactory.Current.Returns(context);

            var uow = new UnitOfWork(dbContextFactory);

            entity = context.Entity1S
                .Include(entity1 => entity1.Entity2!)
                .Single();

            entity.Date1 = DateTimeOffset.UtcNow;

            // Act
            await uow.Save();

            // Assert
            entity.Version
                .Should()
                .Be(2);
        }

        private class DummyDbContext : DbContext
        {
            public DbSet<Entity1> Entity1S { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase("Test");

            protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Entity1>(b => { b.Ignore(p => p.DomainEvents); });
        }

        private sealed class Entity1 : Entity
        {
            public DateTimeOffset Date1 { get; set; }

            public Entity2? Entity2 { get; set; }

            public int Id { get; set; }
        }

        private sealed class Entity2
        {
            public int Id { get; set; }

            internal DateTimeOffset Date2 { get; set; }
        }
    }
}