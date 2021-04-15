using System.Threading.Tasks;

namespace Wiz.Chapter4.Domain.Interfaces.Repository
{
    public interface IDapperReadRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
    }
}
