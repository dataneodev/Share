using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Vero.Shared.Extensions
{
    public static class MapperExtensions
    {
        public static void AddMapper(this IServiceCollection services, Profile profile) =>
            services.AddSingleton(new MapperConfiguration(mc => mc.AddProfile(profile)).CreateMapper());
    }
}