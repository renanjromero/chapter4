using System;

namespace Wiz.Chapter4.Domain.Models
{
    public class Company
    {
        public Company(int id, string domainName, int numberOfEmployees)
        {
            this.Id = id;
            this.DomainName = domainName;
            this.NumberOfEmployees = numberOfEmployees;
        }

        public int Id { get; private set; }
        public string DomainName { get; private set; }
        public int NumberOfEmployees { get; private set; }

        public bool IsEmailCorporate(string email)
        {
            string emailDomain = email.Split('@')[1];
            return DomainName == emailDomain;
        }

        public void ChangeNumberOfEmployees(int delta)
        {
            if(NumberOfEmployees + delta < 0)
                throw new ArgumentException();

            NumberOfEmployees += delta;
        }
    }
}