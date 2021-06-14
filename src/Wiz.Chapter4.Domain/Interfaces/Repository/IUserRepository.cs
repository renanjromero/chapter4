using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.Domain.Interfaces.Repository
{
    public interface IUserRepository: IDapperReadRepository<User>, IEntityBaseRepository<User>
    {
        
    }
}