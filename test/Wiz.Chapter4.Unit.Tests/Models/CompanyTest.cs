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
        public void Diferenciando_emails_corporativos_de_nao_corporativos(string email, bool expectedResult)
        {
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 1); 
            
            bool isEmailCorporate = company.IsEmailCorporate(email);
            
            isEmailCorporate.Should().Be(expectedResult);
        }
    }
}