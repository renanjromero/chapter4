using System.Threading.Tasks;

namespace Wiz.Chapter4.Domain.Interfaces.Services
{
    public interface IMessageBus
    {
        void SendEmailChangedMessage(int userId, string email);
    }
}