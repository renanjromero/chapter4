using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wiz.Chapter4.API.Services;
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
        [Fact]
        public async Task Alteracao_de_um_email_corporativo_para_nao_corporativo()
        {
            //Arrange

            var connectionString = "Server=localhost,1433;Database=chapter;User ID=sa;Password=Q1w2e3r4!;Trusted_Connection=False;";
            var dbContextOptions = new DbContextOptionsBuilder<EntityContext>()
                .UseSqlServer(connectionString)
                .Options;

            var userId = 0;

            using (var entityContext = new EntityContext(dbContextOptions))
            {
                entityContext.Database.EnsureDeleted();
                entityContext.Database.EnsureCreated();

                var dapperContext = new DapperContext(null);
                IUserRepository userRepository = new UserRepository(entityContext, dapperContext);
                ICompanyRepository companyRepository = new CompanyRepository(entityContext, dapperContext);

                var user = new User(email: "user@gmail.com", UserType.Customer);
                var company = new Company(domain: "mycorp.com", numberOfEmployees: 1);

                userRepository.Add(user);
                companyRepository.Add(company);

                entityContext.SaveChanges();

                userId = user.Id;
            }

            //Act

            Mock<IMessageBus> messageBusMock = new Mock<IMessageBus>();
            DomainNotification domainNotification = new DomainNotification();

            using (var entityContext = new EntityContext(dbContextOptions))
            using (var dbConnection = new SqlConnection(connectionString))
            {
                var dapperContext = new DapperContext(dbConnection);

                IUserRepository userRepository = new UserRepository(entityContext, dapperContext);
                ICompanyRepository companyRepository = new CompanyRepository(entityContext, dapperContext);
                UnitOfWork unitOfWork = new UnitOfWork(entityContext);

                var userService = new UserService(
                    userRepository: userRepository,
                    companyRepository: companyRepository,
                    unitOfWork: unitOfWork,
                    messageBus: messageBusMock.Object,
                    domainNotification: domainNotification
                );

                await userService.ChangeEmailAsync(userId, "user@mycorp.com");
            }

            //Assert

            domainNotification.HasNotifications.Should().BeFalse();

            messageBusMock.Verify(x => x.SendEmailChangedMessage(userId, "user@mycorp.com"), Times.Once);

            using (var entityContext = new EntityContext(dbContextOptions))
            using (var dbConnection = new SqlConnection(connectionString))
            {
                var dapperContext = new DapperContext(dbConnection);
                IUserRepository userRepository = new UserRepository(entityContext, dapperContext);
                ICompanyRepository companyRepository = new CompanyRepository(entityContext, dapperContext);

                User user = await userRepository.GetByIdAsync(userId);
                user.Should().NotBeNull();
                user.Email.Should().Be("user@mycorp.com");

                Company company = await companyRepository.GetAsync();
                company.Should().NotBeNull();
                company.NumberOfEmployees.Should().Be(2);
            }
        }
    }
}