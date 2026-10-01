using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vero.Shared.Swagger
{
    public sealed class SchemasDocumentFilter : IDocumentFilter
    {
        private readonly IReadOnlyList<Type> _types;

        public SchemasDocumentFilter(IReadOnlyList<Type> types)
        {
            _types = types;

            if (_types is null)
            {
                _types = Array.Empty<Type>();
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var type in _types)
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            }
        }
    }
}