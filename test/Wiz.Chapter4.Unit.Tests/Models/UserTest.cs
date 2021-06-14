using System;
using FluentAssertions;
using Wiz.Chapter4.Domain.Enums;
using Wiz.Chapter4.Domain.Events;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Notifications;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Models
{
    public class UserTest
    {
        [Fact]
        public void Alteracao_de_um_email_corporativo_para_nao_corporativo()
        {
            User user = new User(id: 10, email: "user@mycorp.com", type: UserType.Employee);
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(4);
            user.EmailChangedEvents.Equals(new EmailChangedEvent(userId: 10, newEmail: "user@gmail.com"));
        }

        [Fact]
        public void Alteracao_de_um_email_nao_corporativo_para_corporativo()
        {
            User user = new User(id: 10, email: "user@gmail.com", type: UserType.Customer);
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@mycorp.com", company);

            user.Email.Should().Be("user@mycorp.com");
            user.Type.Should().Be(UserType.Employee);
            company.NumberOfEmployees.Should().Be(6);
            user.EmailChangedEvents.Equals(new EmailChangedEvent(userId: 1, newEmail: "user@mycorp.com"));
        }

        [Fact]
        public void Alteracao_de_um_email_sem_alterar_o_tipo_de_usuario()
        {
            User user = new User(id: 10, email: "user@gmail.com", type: UserType.Customer);
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@yahoo.com", company);

            user.Email.Should().Be("user@yahoo.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(5);
            user.EmailChangedEvents.Equals(new EmailChangedEvent(userId: 1, newEmail: "user@yahoo.com"));
        }

        [Fact]
        public void Alteracao_de_um_email_para_o_mesmo_email()
        {
            User user = new User(id: 10, email: "user@gmail.com", type: UserType.Customer);
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(5);
            user.EmailChangedEvents.Should().BeEmpty();
        }
    
        [Fact]
        public void Verificando_se_um_email_pode_ser_alterado()
        {
            User user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer, isEmailConfirmed: true);

            NotificationMessage error = user.CanChangeEmail();

            error.Value.Should().Be("O e-mail não pode ser alterado pois já está confirmado");
        } 

        [Fact]
        public void Alterando_um_email_que_nao_pode_ser_alterado()
        {
            User user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer, isEmailConfirmed: true);
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            Assert.Throws<InvalidOperationException>(() => user.ChangeEmail("user@mycorp.com", company));
        }
    }
}