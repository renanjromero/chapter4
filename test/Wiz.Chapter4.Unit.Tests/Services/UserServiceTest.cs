using System.Threading.Tasks;
using Moq;
using Wiz.Chapter4.API.Services;
using Wiz.Chapter4.API.Services.Interfaces;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Models;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMessageBus> _messageBusMock;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _messageBusMock = new Mock<IMessageBus>();
        }

        [Fact]
        public async Task Changing_email_from_non_corporate_to_corporate()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var sut = new UserService
            (
                userRepository: _userRepositoryMock.Object,
                companyRepository: _companyRepositoryMock.Object,
                uow: _uowMock.Object,
                messageBus: _messageBusMock.Object
            );

            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@mycorp.com");

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            _companyRepositoryMock.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
            _uowMock.Verify(x => x.Commit(), Times.Once);

            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@mycorp.com"), Times.Once);
        }

        [Fact]
        public async Task Changing_email_from_corporate_to_non_corporate()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@mycorp.com", UserType.Employee);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var sut = new UserService
            (
                userRepository: _userRepositoryMock.Object,
                companyRepository: _companyRepositoryMock.Object,
                uow: _uowMock.Object,
                messageBus: _messageBusMock.Object
            );

            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@gmail.com");

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            _companyRepositoryMock.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
            _uowMock.Verify(x => x.Commit(), Times.Once);
            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@gmail.com"), Times.Once);
        }

        [Fact]
        public async Task Changing_email_without_changing_user_type()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var sut = new UserService
            (
                userRepository: _userRepositoryMock.Object,
                companyRepository: _companyRepositoryMock.Object,
                uow: _uowMock.Object,
                messageBus: _messageBusMock.Object
            );

            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@outlook.com");

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            _companyRepositoryMock.Verify(x => x.Update(It.IsAny<Company>()), Times.Never);
            _uowMock.Verify(x => x.Commit(), Times.Once);
            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@outlook.com"), Times.Once);
        }

        [Fact]
        public async Task Changing_email_to_the_same_one()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var sut = new UserService
            (
                userRepository: _userRepositoryMock.Object,
                companyRepository: _companyRepositoryMock.Object,
                uow: _uowMock.Object,
                messageBus: _messageBusMock.Object
            );

            await sut.ChangeEmailAsync(userId: 1, newEmail: "user@gmail.com");

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.Update(It.IsAny<Company>()), Times.Never);
            _uowMock.Verify(x => x.Commit(), Times.Never);

            _messageBusMock.Verify(x => x.SendEmailChangedMessage(1, "user@outlook.com"), Times.Never);
        }
    }
}