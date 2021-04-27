using FluentAssertions;
using Wiz.Chapter4.Domain.Models;
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
        }
    }
}