using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vero.Shared.Exceptions;
using Vero.Shared.Extensions;
using Vero.Shared.Security;

namespace Vero.Shared
{
    public static class ContextAccessorExtensions
    {
        public static int? GetUserId(this IEnumerable<Claim> claims) => int.TryParse(
            claims.GetClaim(AccessorClaimType.UserId)
                ?.Value,
            out var id
        )
            ? id
            : null;
        
        public static int? GetTeamId(this IEnumerable<Claim> claims) => int.TryParse(
            claims.GetClaim(AccessorClaimType.TeamId)
                ?.Value,
            out var id
        )
            ? id
            : null;

        public static List<Permission> GetPermission(this IEnumerable<Claim> claims) => claims.GetClaims(AccessorClaimType.Permission)
            .Select(claim => Permission.FromId(int.Parse(claim.Value)))
            .ToList();

        private static Claim? GetClaim(this IEnumerable<Claim> claims, AccessorClaimType type) =>
            claims.SingleOrDefault(claim => claim.Type == type.ToString());

        private static IEnumerable<Claim> GetClaims(this IEnumerable<Claim> claims, AccessorClaimType type) =>
            claims.Where(claim => claim.Type == type.ToString());
    }

    public sealed class ContextAccessor : IContextAccessor
    {
        private readonly ContextAccessorDetails? _details;

        public ContextAccessor(IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext is null)
                return;

            Token = accessor.HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault();

            var teamId = accessor.HttpContext.User.Claims.GetTeamId();
            var userId = accessor.HttpContext.User.Claims.GetUserId();
            var permission = accessor.HttpContext.User.Claims.GetPermission();
            if (userId is null || teamId is null || permission is null )
                return;

            _details = new ContextAccessorDetails(userId.Value, teamId.Value, permission);
        }

        public ContextAccessorDetails Details => _details ?? throw new UnauthorizedAccessException();

        public bool IsSystem => _details is null;

        public string? Token { get; }

        public void CheckPermission(params Permission[] roles)
        {
            if (IsSystem || !roles.Any() || Details.Permission.Has(roles))
                return;

            throw new NoPermissionException();
        }
    }
}