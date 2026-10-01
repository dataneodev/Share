using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public sealed class EntityIdConverter<TEntityId> : ValueConverter<TEntityId, int>
        where TEntityId : EntityId
    {
        public EntityIdConverter(ConverterMappingHints? mappingHints = null) : base(id => id.Value, value => Create(value), mappingHints)
        {
        }

        private static TEntityId Create(int id) => (TEntityId)typeof(TEntityId)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .First()
            .Invoke(new object[] { id });
    }
}