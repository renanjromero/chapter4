using System.Threading.Tasks;
using Wiz.Chapter4.API.Services.Interfaces;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.Services;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IMessageBus messageBus)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
        }

        public async Task ChangeEmailAsync(int userId, string newEmail)
        {
            User user = await _userRepository.GetByIdAsync(userId);

            if(user.Email == newEmail)
                return;

            Company company = await _companyRepository.GetAsync();

            user.ChangeEmail(newEmail, company);

            _userRepository.Update(user);
            _companyRepository.Update(company);
            _unitOfWork.Commit();

            _messageBus.SendEmailChangedMessage(userId, newEmail);
        }
    }
}