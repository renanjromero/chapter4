using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wiz.Chapter4.API.Services;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.Services;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Notifications;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly DomainNotification _domainNotification;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _messageBusMock = new Mock<IMessageBus>();
            _domainNotification = new DomainNotification();
        }

        [Fact]
        public async Task Uma_mensagem_eh_enviada_quando_um_email_eh_alterado()
        {
            //Arrange

            var user = new User(id: 1, email: "user@gmail.com", UserType.Customer);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 1);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _companyRepositoryMock.Setup(x => x.GetAsync())
                .ReturnsAsync(company);

            UserService sut = CreateUserService();

            //Act

            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@mycorp.com");

            //Assert

            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@mycorp.com"), Times.Once);
            _domainNotification.HasNotifications.Should().BeFalse();
        }

        

        [Fact]
        public async Task Um_erro_eh_gerado_se_o_email_nao_pode_ser_alterado()
        {
            //Arrange

            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer, isEmailConfirmed: true);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 1);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);
            
            var sut = CreateUserService();

            //Act
            
            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@mycorp.com");

            //Assert

            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@mycorp.com"), Times.Never);
            _domainNotification.HasNotifications.Should().BeTrue();
        }

        private UserService CreateUserService()
        {
            return new UserService
            (
                userRepository: _userRepositoryMock.Object,
                companyRepository: _companyRepositoryMock.Object,
                unitOfWork: _unitOfWork.Object,
                messageBus: _messageBusMock.Object,
                domainNotification: _domainNotification
            );
        }
    }
}