using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Vero.Shared.EntityFramework
{
    public static class ChangeTrackerExtensions
    {
        public static void ThrowOnMultipleOwnsOneWithTheSameShadowIdObjects(this ChangeTracker changeTracker)
        {
            var invalidOwnedTypes = changeTracker.Entries()
                .Where(w => w.Metadata.IsOwned())
                .GroupBy(g => g.Entity.GetHashCode())
                .Where(w => w.Count() > 1)
                .Where(
                    w => w.SelectMany(
                                 sm => sm.Properties.Where(w => w.Metadata.IsPrimaryKey())
                                     .Select(s => s.CurrentValue)
                             )
                             .Distinct()
                             .Count() ==
                         1
                )
                .Select(
                    s => s.First()
                        .Entity
                )
                .ToArray();

            if (invalidOwnedTypes.Any())
            {
                throw new InvalidOperationException(
                    "Detected multiple (xx) owned object with the same reference and shadow id: " +
                    invalidOwnedTypes.First()
                        .GetType()
                        .FullName
                );
            }
        }
    }
}