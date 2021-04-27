using System;

namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {
        public User(int id, string email, UserType type)
        {
            Id = id;
            Email = email;
            Type = type;
        }

        public int Id { get; set; }

        public string Email { get; set; }

        public UserType Type { get; set; }

        public void ChangeEmail(string newEmail, Company company)
        {
            string emailDomain = newEmail.Split('@')[1];
            UserType newType = emailDomain == company.Domain ? UserType.Employee : UserType.Customer;

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