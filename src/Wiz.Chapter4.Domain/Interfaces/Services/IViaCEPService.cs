using System.Threading.Tasks;
using Wiz.Chapter4.Domain.Models.Services;

namespace Wiz.Chapter4.Domain.Interfaces.Services
{
    public interface IViaCEPService
    {
        Task<ViaCEP> GetByCEPAsync(string cep);
    }
}
