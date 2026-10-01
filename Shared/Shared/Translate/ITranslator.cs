using Vero.Shared.ValueObjects;

namespace Vero.Shared.Translate
{
    public interface ITranslator
    {
        Task InitAsync(IReadOnlyList<int> ids, AppLanguage language);

        string? GetName(int id, AppLanguage language);
    }
}
