namespace Vero.Shared.Security
{
    public enum AccessorClaimType
    {
        UserId = 1,
        Permission = 2,
        TeamId = 3
    }

    public interface IContextAccessor
    {
        public ContextAccessorDetails Details { get; }

        public bool IsSystem { get; }

        public string? Token { get; }

        public void CheckPermission(params Permission[] roles);
    }

    public sealed class ContextAccessorDetails
    {
        public ContextAccessorDetails(int userId, int teamId, IReadOnlyList<Permission> permission)
        {
            UserId = userId;
            TeamId = teamId;
            Permission = permission;
        }

        public int UserId { get; }

        public int TeamId { get; }

        public IReadOnlyList<Permission> Permission { get; }
    }
}