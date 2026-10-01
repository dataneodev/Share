using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Vero.Shared.Security
{
    public static class Security
    {
        public const string Issuer = "Vero";

        public const string Audience = "Vero";

        public const string AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;

        public static readonly SymmetricSecurityKey SymmetricSecurityKey = new(
            Encoding.ASCII.GetBytes(
                "!RCf12LaseUKjHj7C1xGP4JSzVHZVUvBbjx3BLj1L33C@kSZw0daryO8U3Y$7B3f#mrG#@pmpHGzjSVgYkknUCbZZmOI30DIaje!MBhnZHQCWsC7ARbtv@!a75Oe0ddkyHtCufzkjPBBCVkHFKtCEC#6N9gPbFWf#oc3OyvP78IiATgB5wc6!6Tui3D$ABTxZ0yztAeLCMmKOWAMhvukWsXYtl7mxBK1MP4RodpEzVQTsjFCQz#1XUuS4w8GmBD3"
            )
        );
    }
}