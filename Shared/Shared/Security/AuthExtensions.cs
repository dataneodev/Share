using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Vero.Shared.Security
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, AuthOptions? auth)
        {
            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultChallengeScheme = Security.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = Security.AuthenticationScheme;
                    }
                )
                .AddJwtBearer(
                    options =>
                    {
                        if (auth?.Configure is not null)
                            options = auth.Configure(options);

                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = Security.Issuer,
                            ValidateAudience = true,
                            ValidAudience = Security.Audience,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = Security.SymmetricSecurityKey,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    }
                );

            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, AuthOptions? options = null)
        {
            services.AddAppAuthentication(options);
            services.AddAuthorization();
            return services;
        }

        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}