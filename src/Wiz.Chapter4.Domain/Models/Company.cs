using System;
using Wiz.Chapter4.Domain.Validations;

namespace Wiz.Chapter4.Domain.Models
{
    public class Company
    {
        public Company(string domain, int numberOfEmployees)
        {
            Domain = domain;
            NumberOfEmployees = numberOfEmployees;
        }

        public string Domain { get; private set; }

        public int NumberOfEmployees { get; private set; }

        public bool IsEmailCorporate(string email)
        {
            string emailDomain = email.Split('@')[1];
            return emailDomain == Domain;
        }

        public void ChangeNumberOfEmployees(int delta)
        {
            Precondition.Requires(NumberOfEmployees + delta >= 0);

            NumberOfEmployees += delta;
        }
    }
}