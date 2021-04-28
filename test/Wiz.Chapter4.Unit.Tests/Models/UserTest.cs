using System;
using FluentAssertions;
using src.Wiz.Chapter4.Domain.Events;
using Wiz.Chapter4.Domain.Models;
using Wiz.Chapter4.Domain.Notifications;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Models
{
    public class UserTest
    {
        [Fact]
        public void Alterando_um_email_de_corporativo_para_nao_corporativo()
        {
            var user = new User(id: 1, email: "user@mycorp.com", type: UserType.Employee);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(4);
            user.EmailChangedEvents.Should().Equal(new EmailChangedEvent(userId: 1, newEmail: "user@gmail.com"));
        }

        [Fact]
        public void Alterando_um_email_de_nao_corporativo_para_corporativo()
        {
            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@mycorp.com", company);

            user.Email.Should().Be("user@mycorp.com");
            user.Type.Should().Be(UserType.Employee);
            company.NumberOfEmployees.Should().Be(6);
            user.EmailChangedEvents.Count.Should().Be(1);
        }

        [Fact]
        public void Alterando_um_email_sem_alterar_o_tipo_do_usuario()
        {
            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@outlook.com", company);

            user.Email.Should().Be("user@outlook.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(5);
            user.EmailChangedEvents.Count.Should().Be(1);
        }

        [Fact]
        public void Alterando_um_email_para_o_mesmo_existente()
        {
            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(5);
            user.EmailChangedEvents.Count.Should().Be(0);
        }

        [Fact]
        public void Verificando_se_um_email_pode_ser_alterado()
        {
            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer, isEmailConfirmed: true);

            NotificationMessage error = user.CanChangeEmail();

            error.Should().NotBeNull();
        }

        [Fact]
        public void Alterando_um_email_que_nao_pode_ser_alterado()
        {
            var user = new User(id: 1, email: "user@gmail.com", type: UserType.Customer, isEmailConfirmed: true);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            Assert.Throws<InvalidOperationException>(() => user.ChangeEmail("user@mycorp.com", company));
        }
    }
}