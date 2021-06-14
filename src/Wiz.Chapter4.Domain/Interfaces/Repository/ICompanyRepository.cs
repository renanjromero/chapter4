using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.Domain.Interfaces.Repository
{
    public interface ICompanyRepository: IEntityBaseRepository<Company>
    {
        Task<Company> GetAsync();
    }
}