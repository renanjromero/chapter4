using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Wiz.Chapter4.API.ViewModels.Customer;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Models.Dapper;

namespace Wiz.Chapter4.API.AutoMapper
{
    [ExcludeFromCodeCoverage]
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Customer

            CreateMap<CustomerAddress, CustomerAddressViewModel>()
                .ConstructUsing(s => new CustomerAddressViewModel(
                    s.Id,
                    s.AddressId, 
                    s.Name, 
                    s.DateCreated, 
                    s.CEP, 
                    null));
            CreateMap<Customer, CustomerViewModel>()
                .ConstructUsing(s=> new CustomerViewModel(
                    s.Id,
                    s.AddressId,
                    s.Name
                )).ReverseMap();

            #endregion
        }
    }
}
