using System.Collections.Generic;
using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Models.Dapper;

namespace Wiz.Chapter4.Domain.Interfaces.Repository
{
    public interface ICustomerRepository : IEntityBaseRepository<Customer>, IDapperReadRepository<Customer>
    {
        Task<IEnumerable<CustomerAddress>> GetAllAsync();
        Task<CustomerAddress> GetAddressByIdAsync(int id);
        Task<CustomerAddress> GetByNameAsync(string name);
    }
}
