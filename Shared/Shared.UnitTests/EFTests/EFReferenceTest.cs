using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vero.Shared.DDD;
using Vero.Shared.EntityFramework;
using Vero.Shared.EntityFramework.Converters;
using Vero.Shared.ValueObjects;
using Xunit;

namespace Shared.UnitTests.EFTests
{
    //https://github.com/dotnet/efcore/issues/19856
    public sealed class EFReferenceTest
    {
        [Fact]
        public async Task ef_should_throw_when_same_reference_one_nip_reference()
        {
            var nip = new Nip(Country.PL, "4562423432");

            var nipReference = new NipReference { Id = 1, First = nip, Last = nip };

            var db = GetMemoryContext();

            db.NipReferences.Add(nipReference);

            Func<Task> act = async () => await db.SaveChangesAsync();

            await act.Should()
                .ThrowAsync<InvalidOperationException>();

            //Cannot save instance of 'NipReference.Last#Nip' because it is an owned entity without any reference to its owner. Owned entities can only be saved as part of an aggregate also including the owner entity.

            //await db.SaveChangesAsync();
            //Assert.True(true);
        }

        [Fact]
        public async Task ef_should_throw_when_same_reference_own_one_reference()
        {
            var nipFirst = new Nip(Country.PL, "4562423432");
            var nipLast = new Nip(Country.PL, "4562423432");

            var nipReference1 = new NipReference { Id = 1, First = nipFirst, Last = nipLast };
            var nipReference2 = new NipReference { Id = 2, First = nipFirst, Last = nipLast };

            var db = GetMemoryContext();

            db.NipReferences.Add(nipReference1);
            db.NipReferences.Add(nipReference2);

            var act = () => db.ChangeTracker.ThrowOnMultipleOwnsOneWithTheSameShadowIdObjects();

            act.Should()
                .Throw<InvalidOperationException>();

            // await db.SaveChangesAsync();
            //
            // var nips = await db.NipReferences.ToArrayAsync();
            //
            // nips.Should()
            //     .AllSatisfy(
            //         a => a.First.Should()
            //             .NotBeNull()
            //     );
            //
            // nips.Should()
            //     .AllSatisfy(
            //         a => a.Last.Should()
            //             .NotBeNull()
            //     );
        }

        [Fact]
        public async Task ef_should_throw_when_same_reference_value_object_reference()
        {
            var intValue = new TestValuOfInt(1);

            var nipReference1 = new NipReference
            {
                Id = 1, First = new Nip(Country.PL, "4562423432"), Last = new Nip(Country.PL, "4562423432"), ValueObjInt = intValue
            };

            var nipReference2 = new NipReference
            {
                Id = 2, First = new Nip(Country.PL, "4562423432"), Last = new Nip(Country.PL, "4562423432"), ValueObjInt = intValue
            };

            var db = GetMemoryContext();

            db.NipReferences.Add(nipReference1);
            db.NipReferences.Add(nipReference2);

            var act = () => db.ChangeTracker.ThrowOnMultipleOwnsOneWithTheSameShadowIdObjects();

            act.Should()
                .NotThrow<InvalidOperationException>();
        }

        [Fact]
        public async Task ef_should_not_throw_when_different_owns_one_reference()
        {
            var nipFirst = new Nip(Country.PL, "4562423432");
            var nipLast = new Nip(Country.PL, "4562423432");
            var nipReference = new NipReference { Id = 1, First = nipFirst, Last = nipLast };

            var db = GetMemoryContext();

            db.NipReferences.Add(nipReference);

            Func<Task> act = async () => await db.SaveChangesAsync();
            await act.Should()
                .NotThrowAsync<InvalidOperationException>();
        }

        public NipReferenceContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<NipReferenceContext>().UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            return new NipReferenceContext(options);
        }
    }

    public sealed class NipReference
    {
        public int Id { get; set; }

        public Nip First { get; set; }

        public Nip Last { get; set; }

        public TestValuOfInt? ValueObjInt { get; set; }
    }

    public sealed record TestValuOfInt(int Value) : ValueObjectOf<int>(Value);

    public class NipReferenceContext : DbContext
    {
        public NipReferenceContext()
        {
        }

        public NipReferenceContext(DbContextOptions<NipReferenceContext> options) : base(options)
        {
        }

        public DbSet<NipReference> NipReferences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NipReference>()
                .Property(p => p.Id);

            modelBuilder.Entity<NipReference>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<NipReference>()
                .OwnsOne(p => p.First, c => c.NipProperty("first_value", "first_currency"));

            modelBuilder.Entity<NipReference>()
                .OwnsOne(p => p.Last, c => c.NipProperty("last_value", "last_currency"));


            modelBuilder.Entity<NipReference>()
                .Property(p => p.ValueObjInt)
                .HasIntConversion();
        }
    }
}