using System;
using System.Collections.Generic;
using src.Wiz.Chapter4.Domain.Events;
using Wiz.Chapter4.Domain.Notifications;

namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {
        public User(int id, string email, UserType type)
        {
            Id = id;
            Email = email;
            Type = type;
            EmailChangedEvents = new List<EmailChangedEvent>();
        }

        public User(int id, string email, UserType type, bool isEmailConfirmed) : this(id, email, type)
        {
            IsEmailConfirmed = isEmailConfirmed;
        }

        public int Id { get; private set; }

        public string Email { get; private set; }

        public UserType Type { get; private set; }

        public bool IsEmailConfirmed { get; private set; }

        public ICollection<EmailChangedEvent> EmailChangedEvents { get; private set; }

        public void ChangeEmail(string newEmail, Company company)
        {        
            if(CanChangeEmail() != null)
                throw new InvalidOperationException();                

            if(Email == newEmail)
                return;

            UserType newType = company.IsEmailCorporate(newEmail) ? UserType.Employee : UserType.Customer;

            if (Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                company.NumberOfEmployees = company.NumberOfEmployees + delta;
            }

            Email = newEmail;
            Type = newType;       

            EmailChangedEvents.Add(new EmailChangedEvent(userId: Id, newEmail: newEmail));
        }

        public NotificationMessage CanChangeEmail()
        {
            if(IsEmailConfirmed){
                return new NotificationMessage("","O e-mail não pode ser alterado pois já está confirmado");
            }

            return null;
        }
    }
}