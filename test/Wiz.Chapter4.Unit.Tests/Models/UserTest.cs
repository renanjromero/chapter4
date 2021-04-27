using FluentAssertions;
using Wiz.Chapter4.Domain.Models;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Models
{
    public class UserTest
    {
        [Fact]
        public void Alterando_um_email_corporativo_para_nao_corporativo()
        {
            var user = new User(id: 1, email: "user@mycorp.com", type: UserType.Employee);
            var company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(4);
        }
    }
}