using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.Domain.Interfaces.Repository
{
    public interface IUserRepository: IEntityBaseRepository<User>
    {
        Task<User> GetByIdAsync(int userId);
    }
}