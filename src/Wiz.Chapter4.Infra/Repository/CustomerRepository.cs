﻿using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Models.Dapper;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.Repository
{
    public class CustomerRepository : EntityBaseRepository<Customer>, ICustomerRepository
    {
        private readonly DapperContext _dapperContext;

        public CustomerRepository(EntityContext context, DapperContext dapperContext)
            : base(context)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<CustomerAddress>> GetAllAsync()
        {
            var query = @"SELECT c.Id AS id, a.Id AS addressId, c.Name AS name, c.DateCreated AS dateCreated, a.CEP AS cep
                            FROM dbo.Customer c
                            INNER JOIN dbo.Address a
                            ON c.addressId = a.Id";

            return await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query,null,null,null,null);
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var query = @"SELECT Id, AddressId, Name, DateCreated
                          FROM dbo.Customer c
                          WHERE c.Id = @Id";

            return (await _dapperContext.DapperConnection.QueryAsync<Customer>(query, new { Id = id })).FirstOrDefault();
        }

        public async Task<CustomerAddress> GetAddressByIdAsync(int id)
        {
            var query = @"SELECT c.Id, a.Id AS AddressId, c.Name, c.DateCreated, a.CEP
                          FROM dbo.Customer c
                          INNER JOIN dbo.Address a
                          ON c.AddressId = a.Id
                          WHERE c.Id = @Id";

            return (await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query, new { Id = id })).FirstOrDefault();
        }

        public async Task<CustomerAddress> GetByNameAsync(string name)
        {
            var query = @"SELECT c.Id AS id, a.Id AS addressId, c.Name AS name, c.DateCreated AS dateCreated, a.CEP AS cep
                          FROM dbo.Customer c
                          INNER JOIN dbo.Address a
                          ON c.addressId = a.Id
                          WHERE c.Name = @Name";

            return (await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query, new { Name = name })).FirstOrDefault();
        }
    }
}
