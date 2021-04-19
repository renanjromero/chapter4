using FluentAssertions;
using Wiz.Chapter4.Domain.Models;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Models
{
    public class UserTest
    {
        [Fact]
        public void Changing_email_from_non_corporate_to_corporate()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            user.ChangeEmail("user@mycorp.com", company);

            user.Email.Should().Be("user@mycorp.com");
            user.Type.Should().Be(UserType.Employee);
            company.NumberOfEmployees.Should().Be(2);
        }

        [Fact]
        public void Changing_email_from_corporate_to_non_corporate()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@mycorp.com", UserType.Employee);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(0);
        }

        [Fact]
        public void Changing_email_without_changing_user_type()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            user.ChangeEmail("user@outlook.com", company);

            user.Email.Should().Be("user@outlook.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(1);
        }

        [Fact]
        public void Changing_email_to_the_same_one()
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);
            var user = new User(id: 1, companyId: 1, email: "user@gmail.com", UserType.Customer);

            user.ChangeEmail("user@gmail.com", company);

            user.Email.Should().Be("user@gmail.com");
            user.Type.Should().Be(UserType.Customer);
            company.NumberOfEmployees.Should().Be(1);
        }
    }
}