using FluentAssertions;
using Wiz.Chapter4.Domain.Models;
using Xunit;

namespace test.Wiz.Chapter4.Unit.Tests.Models
{
    public class CompanyTest
    {
        [Theory]
        [InlineData("user@mycorp.com", true)]
        [InlineData("user@gmail.com", false)]
        public void Company_can_differ_corporate_from_no_corporate_emails(string newEmail, bool expectedResult)
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: 1);

            bool isCorporate = company.IsEmailCorporate(newEmail);

            isCorporate.Should().Be(expectedResult);
        }
    }
}