using System;

namespace Wiz.Chapter4.Domain.Models
{
    public class Company
    {
        public Company(string domain, int numberOfEmployees)
        {
            Domain = domain;
            NumberOfEmployees = numberOfEmployees;
        }

        private string Domain { get; set; }

        public int NumberOfEmployees { get; set; }

        public bool IsEmailCorporate(string email)
        {
            string emailDomain = email.Split('@')[1];
            return Domain == emailDomain;
        }
    }
}