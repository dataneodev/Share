using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Vero.Shared.Swagger
{
    public static class SwaggerExtensions
    {
        private static string _name = string.Empty;

        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services,
            HashSet<Type>? swaggerTypesForPolymorphism,
            AssembliesContainer assemblies
        )
        {
            _name = Assembly.GetEntryAssembly()
                ?.GetName()
                .Name!;


            services.AddSwaggerGen(
                options =>
                {
                    if ((swaggerTypesForPolymorphism?.Count ?? 0) > 0)
                    {
                        options.UseAllOfForInheritance();
                        options.UseOneOfForPolymorphism();

                        options.SelectDiscriminatorNameUsing(baseType => "typeId");
                        options.SelectDiscriminatorValueUsing(subType => GetPropertyValue(subType, "TypeId"));

                        options.SelectSubTypesUsing(
                            baseType =>
                            {
                                if (!swaggerTypesForPolymorphism!.Contains(baseType))
                                    return Enumerable.Empty<Type>();

                                return assemblies.Infrastructure?.GetTypes()
                                           .Where(type => type.IsSubclassOf(baseType)) ??
                                       Enumerable.Empty<Type>();
                            }
                        );
                    }

                    options.CustomSchemaIds(x => x.FullName);

                    options.SwaggerDoc(
                        _name,
                        new OpenApiInfo
                        {
                            Title = _name,
                            Version = Assembly.GetEntryAssembly()
                                ?.GetCustomAttribute<AssemblyFileVersionAttribute>()
                                ?.Version
                        }
                    );

                    options.AddSecurityDefinition(
                        Security.Security.AuthenticationScheme,
                        new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            Scheme = Security.Security.AuthenticationScheme,
                            Description = $"Please enter into field the word '{Security.Security.AuthenticationScheme}' following by space and JWT"
                        }
                    );

                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = Security.Security.AuthenticationScheme },
                                    Scheme = "oauth2",
                                    Name = Security.Security.AuthenticationScheme,
                                    In = ParameterLocation.Header
                                },
                                new List<string>()
                            }
                        }
                    );

                    options.EnableAnnotations();
                }
            );

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{_name}/swagger.json", _name); });

            return app;
        }

        private static string GetPropertyValue(Type type, string constName)
        {
            const string defaultValue = "0";
            return type?.GetProperty(constName, BindingFlags.Public | BindingFlags.Instance)
                       ?.GetValue(Activator.CreateInstance(type, true))
                       ?.ToString() ??
                   defaultValue;
        }
    }
}