using System.Threading.Tasks;
using Dapper;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.Repository
{
    public class CompanyRepository: EntityBaseRepository<Company>, ICompanyRepository
    {
        private readonly DapperContext _dapperContext;

        public CompanyRepository(EntityContext entityContext, DapperContext dapperContext): base(entityContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Company> GetAsync()
        {
            var query = @"
                SELECT
                    *
                FROM [dbo].[Company]";

            return await _dapperContext.DapperConnection.QueryFirstOrDefaultAsync<Company>(query, new { });
        }
    }
}