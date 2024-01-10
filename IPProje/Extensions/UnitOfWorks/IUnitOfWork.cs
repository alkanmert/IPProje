using IPProje.EntityBase;
using IPProje.Extensions.Repositories;

namespace IPProje.Extensions.UnitOfWorks
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity, new();
        Task<int> SaveAsync();
        int Save();
    }
}
