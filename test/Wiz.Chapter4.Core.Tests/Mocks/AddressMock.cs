using Bogus;
using Bogus.Extensions.Brazil;
using Wiz.Chapter4.API.ViewModels.Address;
using Wiz.Chapter4.API.ViewModels.Customer;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Models.Dapper;

namespace Wiz.Chapter4.Core.Tests.Mocks
{
    public static class AddressMock
    {
        public static Faker<AddressViewModel> AddressViewModelFaker =>
            new Faker<AddressViewModel>("pt_BR")
            .CustomInstantiator(x => new AddressViewModel
            (
                id: x.Random.Number(1, 10),
                cep: x.Address.ZipCode(),
                street: x.Address.StreetName(),
                streetFull: x.Address.StreetAddress(),
                uf: x.Address.State()
            ));
    }
}
