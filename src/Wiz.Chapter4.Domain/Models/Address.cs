using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wiz.Chapter4.Domain.Models
{
    public class Address
    {
        public Address()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; private set; }
        public string CEP { get; private set; }

        public ICollection<Customer> Customers { get; private set; }

        public Address AddCep(string cep){
            this.CEP = cep;
            return this;
        }
    }
}
