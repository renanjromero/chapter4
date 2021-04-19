using System;

namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {

        public User(int id, int companyId, string email, UserType type)
        {
            this.Id = id;
            this.CompanyId = companyId;
            this.Email = email;
            this.Type = type;
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }

        public void ChangeEmail(string newEmail, Company company)
        {
            UserType newType = company.IsEmailCorporate(newEmail) ? UserType.Employee : UserType.Customer;

            if (Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                company.NumberOfEmployees = company.NumberOfEmployees + delta;
            }

            Email = newEmail;
            Type = newType;
        }
    }
}