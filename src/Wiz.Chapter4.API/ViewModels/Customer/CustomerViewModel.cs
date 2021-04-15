﻿using System;
using Newtonsoft.Json;
using Wiz.Chapter4.API.ViewModels.Address;

namespace Wiz.Chapter4.API.ViewModels.Customer
{
    public class CustomerViewModel
    {
        [JsonConstructor]
        public CustomerViewModel(int id, int addressId, string name)
        {
            Id = id;
            AddressId = addressId;
            Name = name;
        }

        public int Id { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public AddressViewModel Address { get; set; }
        
        
    }
}
