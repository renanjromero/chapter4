using System;
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

        [Theory]
        [InlineData(5, 1, 6)]
        [InlineData(3, 2, 5)]  
        [InlineData(1, -1, 0)]  
        public void Number_of_employees_is_changed(int initial, int delta, int expected)
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: initial);

            company.ChangeNumberOfEmployees(delta);

            company.NumberOfEmployees.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, -5)]
        [InlineData(1, -2)]
        public void Number_of_employees_must_not_be_negative(int initial, int delta)
        {
            var company = new Company(id: 1, domainName: "mycorp.com", numberOfEmployees: initial);

            Assert.Throws<ArgumentException>(() => company.ChangeNumberOfEmployees(delta: delta));
        }
    }
}