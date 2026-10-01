namespace Vero.Shared.Extensions
{
    public static class DIExtensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            var service = (T)provider.GetService(typeof(T))! ?? throw new ApplicationException($"Nie udało się załadować modułu {typeof(T).FullName}");
            return service;
        }
    }
}