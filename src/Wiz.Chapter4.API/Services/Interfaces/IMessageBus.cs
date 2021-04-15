using Wiz.Chapter4.Domain.Interfaces.Repository;

namespace Wiz.Chapter4.API.Services.Interfaces
{
    public interface IMessageBus
    {
        void SendEmailChangedMessage(int userId, string newEmail);
    }
}