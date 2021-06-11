using System;
using System.Collections.Generic;
using Wiz.Chapter4.Domain.Enums;
using Wiz.Chapter4.Domain.Notifications;
using Wiz.Chapter4.Domain.Validations;

namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {
        public User(int id, string email, UserType type): this(id, email, type, isEmailConfirmed: false)
        {            
        }

        public User(int id, string email, UserType type, bool isEmailConfirmed)
        {
            Id = id;
            Email = email;
            Type = type;
            IsEmailConfirmed = isEmailConfirmed;

            EmailChangedEvents = new List<EmailChangedEvent>();
        }

        public int Id { get; private set; }

        public string Email { get; private set; }

        public UserType Type { get; private set; }

        public bool IsEmailConfirmed { get; private set; }

        public List<EmailChangedEvent> EmailChangedEvents { get; private set; }

        public void ChangeEmail(string newEmail, Company company)
        {
            if(Email == newEmail)
                return;

            Precondition.Requires(CanChangeEmail() == null);

            string emailDomain = newEmail.Split('@')[1];
            UserType newType = emailDomain == company.Domain ? UserType.Employee : UserType.Customer;

            if (Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                company.ChangeNumberOfEmployees(delta);
            }

            Email = newEmail;
            Type = newType;

            EmailChangedEvents.Add(new EmailChangedEvent(Id, newEmail));
        }

        public NotificationMessage CanChangeEmail()
        {
            if(IsEmailConfirmed)
            {
                return new NotificationMessage("","O e-mail não pode ser alterado pois já está confirmado");
            }

            return null;
        }
    }

    public struct EmailChangedEvent
    {
        public EmailChangedEvent(int userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }

        public int UserId { get; }

        public string NewEmail { get; }
    }
}   