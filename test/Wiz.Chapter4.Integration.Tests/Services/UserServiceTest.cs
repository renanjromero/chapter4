using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wiz.Chapter4.API.Services;
using Wiz.Chapter4.API.Services.Interfaces;
using Wiz.Chapter4.Domain.Enums;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.Services;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Notifications;
using Wiz.Chapter4.Infra.Context;
using Wiz.Chapter4.Infra.Repository;
using Wiz.Chapter4.Infra.UoW;
using Xunit;

namespace Wiz.Chapter4.Integration.Tests.Services
{
    public class UserServiceTest
    {
        private readonly string _connectionString;
        private readonly DbContextOptions<EntityContext> _dbContextOptions;

        public UserServiceTest()
        {
            _connectionString = "Server=localhost,1433;Database=chapter;User ID=sa;Password=Q1w2e3r4!;Trusted_Connection=False;";
            _dbContextOptions = new DbContextOptionsBuilder<EntityContext>()
               .UseSqlServer(_connectionString)
               .Options;

        }

        [Fact]
        public async Task Alteracao_de_um_email_corporativo_para_nao_corporativo()
        {
            //Arrange

            CreateDatabase();

            User user = CreateUser(email: "user@gmail.com", type: UserType.Customer);
            CreateCompany(domain: "mycorp.com", numberOfEmployees: 1);

            Mock<IMessageBus> messageBusMock = new Mock<IMessageBus>();
            DomainNotification domainNotification = new DomainNotification();

            //Act

            await Execute(
                action: async x => await x.ChangeEmailAsync(user.Id, "user@mycorp.com"),
                messageBusMock.Object, domainNotification
            );

            //Assert

            domainNotification.HasNotifications.Should().BeFalse();

            User userFromDb = await QueryUser(user.Id);
            userFromDb.Should().NotBeNull();
            userFromDb.Email.Should().Be("user@mycorp.com");

            Company companyFromDb = await QueryCompany();
            companyFromDb.Should().NotBeNull();
            companyFromDb.NumberOfEmployees.Should().Be(2);

            messageBusMock.Verify(x => x.SendEmailChangedMessage(user.Id, "user@mycorp.com"), Times.Once);
        }

        private void CreateDatabase()
        {
            using (var entityContext = new EntityContext(_dbContextOptions))
            {
                entityContext.Database.EnsureDeleted();
                entityContext.Database.EnsureCreated();
            }
        }

        private User CreateUser(
            string email = "user@mycorp.com",
            UserType type = UserType.Customer,
            bool isEmailConfirmed = false)
        {
            using (var entityContext = new EntityContext(_dbContextOptions))
            {
                var dapperContext = new DapperContext(null);
                IUserRepository userRepository = new UserRepository(entityContext, dapperContext);

                var user = new User(email, type, isEmailConfirmed);

                userRepository.Add(user);
                entityContext.SaveChanges();

                return user;
            }
        }

        private async Task<User> QueryUser(int userId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                var dapperContext = new DapperContext(dbConnection);
                IUserRepository userRepository = new UserRepository(null, dapperContext);

                return await userRepository.GetByIdAsync(userId);
            }
        }

        private void CreateCompany(
            string domain = "mycorp.com",
            int numberOfEmployees = 1)
        {
            using (var entityContext = new EntityContext(_dbContextOptions))
            {
                var dapperContext = new DapperContext(null);
                ICompanyRepository companyRepository = new CompanyRepository(entityContext, dapperContext);

                var company = new Company(domain, numberOfEmployees);

                companyRepository.Add(company);

                entityContext.SaveChanges();
            }
        }

        private async Task<Company> QueryCompany()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                var dapperContext = new DapperContext(dbConnection);
                ICompanyRepository companyRepository = new CompanyRepository(null, dapperContext);

                return await companyRepository.GetAsync();
            }
        }

        private async Task Execute(
            Func<IUserService, Task> action,
            IMessageBus messageBus,
            DomainNotification domainNotification)
        {
            using (var entityContext = new EntityContext(_dbContextOptions))
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                var dapperContext = new DapperContext(dbConnection);

                IUserRepository userRepository = new UserRepository(entityContext, dapperContext);
                ICompanyRepository companyRepository = new CompanyRepository(entityContext, dapperContext);
                UnitOfWork unitOfWork = new UnitOfWork(entityContext);

                var userService = new UserService(
                    userRepository: userRepository,
                    companyRepository: companyRepository,
                    unitOfWork: unitOfWork,
                    messageBus: messageBus,
                    domainNotification: domainNotification
                );

                await action(userService);
            }
        }
    }
}