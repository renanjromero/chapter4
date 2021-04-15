﻿using System;
using Newtonsoft.Json;
using Wiz.Chapter4.API.ViewModels.Address;

namespace Wiz.Chapter4.API.ViewModels.Customer
{
    public class CustomerAddressViewModel
    {
        [JsonConstructor]
        public CustomerAddressViewModel(int id, int addressId, string name, DateTime dateCreated, string cep, AddressViewModel address)
        {
            Id = id;
            AddressId = addressId;
            Name = name;
            DateCreated = dateCreated;
            CEP = cep;
            Address = address ?? new AddressViewModel(addressId, cep,null,null,null);
        }

        public int Id { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string CEP { get; set; }
        public AddressViewModel Address { get; set; }
    }
}
