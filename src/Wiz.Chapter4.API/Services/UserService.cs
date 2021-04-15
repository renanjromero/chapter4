using System.Threading.Tasks;
using Wiz.Chapter4.API.Services.Interfaces;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMessageBus _messageBus;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository, IUnitOfWork uow, IMessageBus messageBus)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _uow = uow;
            _messageBus = messageBus;
        }

        public async Task ChangeEmailAsync(int userId, string newEmail)
        {
            User user = await _userRepository.GetByIdAsync(userId);

            if(user.Email == newEmail)
            {
                return;
            }

            Company company = await _companyRepository.GetByIdAsync(user.CompanyId);

            string emailDomain = newEmail.Split('@')[1];
            UserType newType = emailDomain == company.DomainName ? UserType.Employee : UserType.Customer;

            if (user.Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                company.NumberOfEmployees = company.NumberOfEmployees + delta;
                _companyRepository.Update(company);
            }

            user.Email = newEmail;
            user.Type = newType;

            _userRepository.Update(user);

            _uow.Commit();

            _messageBus.SendEmailChangedMessage(userId, newEmail);
        }
    }
}