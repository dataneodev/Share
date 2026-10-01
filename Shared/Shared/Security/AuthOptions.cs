using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Vero.Shared.Security
{
    public sealed class AuthOptions
    {
        public Func<JwtBearerOptions, JwtBearerOptions> Configure { get; init; }
    }
}