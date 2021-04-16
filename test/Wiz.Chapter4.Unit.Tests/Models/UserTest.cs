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

            user.ChangeEmail("user@corp.com", company);

            user.Email.Should().Be("user@corp.com");
            user.Type.Should().Be(UserType.Employee);
            company.NumberOfEmployees.Should().Be(2);
        }
    }
}