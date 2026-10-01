using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vero.Shared.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        private static bool IsHandled(IMutableEntityType w, Type baseType)
        {
            return !w.IsAbstract() && !w.IsOwned() && IsBaseType(w.ClrType?.BaseType, baseType);
        }

        private static bool IsBaseType(Type? baseType, Type find)
        {
            if (baseType is null)
                return false;

            if (baseType == find)
                return true;

            return IsBaseType(baseType.BaseType, find);
        }

        public static EntityTypeBuilder UseXminAsConcurrencyToken(this EntityTypeBuilder entityTypeBuilder)
        {
            entityTypeBuilder.Property<uint>("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            return entityTypeBuilder;
        }
    }
}