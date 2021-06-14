using System.Threading.Tasks;
using Wiz.Chapter4.API.Services.Interfaces;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Interfaces.Services;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Notifications;

namespace Wiz.Chapter4.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;
        private readonly DomainNotification _domainNotification;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IMessageBus messageBus, DomainNotification domainNotification)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
            _domainNotification = domainNotification;
        }

        public async Task ChangeEmailAsync(int userId, string newEmail)
        {
            User user = await _userRepository.GetByIdAsync(userId);

            if(user.Email == newEmail)
                return;

            var notificationMessage = user.CanChangeEmail();
            if(notificationMessage != null)
            {
                _domainNotification.AddNotifications(new []{notificationMessage});
                return;
            }

            Company company = await _companyRepository.GetAsync();

            user.ChangeEmail(newEmail, company);

            _userRepository.Update(user);
            _companyRepository.Update(company);
            _unitOfWork.Commit();

            foreach (var emailChangedEvent in user.EmailChangedEvents)
            {
                _messageBus.SendEmailChangedMessage(emailChangedEvent.UserId, emailChangedEvent.NewEmail);
            }
        }
    }
}