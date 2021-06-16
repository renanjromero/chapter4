using Dapper;
using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.Repository
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        private readonly DapperContext _dapperContext;

        public UserRepository(EntityContext entityContext, DapperContext dapperContext) : base(entityContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var query = @"
                SELECT
                    *
                FROM [dbo].[User]
                WHERE Id = @id";

            return await _dapperContext.DapperConnection.QueryFirstOrDefaultAsync<User>(query, new { id });
        }
    }
}