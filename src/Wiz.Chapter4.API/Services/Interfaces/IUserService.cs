using System.Threading.Tasks;

namespace Wiz.Chapter4.API.Services.Interfaces
{
    public interface IUserService
    {
        Task ChangeEmailAsync(int userId, string newEmail);
    }
}