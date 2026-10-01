namespace Vero.Shared.Abstractions
{
    public interface IUnitOfWork
    {
        Task Save(CancellationToken cancellationToken);
    }
}
