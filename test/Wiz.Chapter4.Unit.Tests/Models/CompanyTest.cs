using System;
using FluentAssertions;
using Wiz.Chapter4.Domain.Models;
using Xunit;

namespace Wiz.Chapter4.Unit.Tests.Models
{
    public class CompanyTest
    {
        [Theory]
        [InlineData("user@mycorp.com", true)]
        [InlineData("user@gmail.com", false)]
        public void Diferenciacao_entre_email_corporativo_e_nao_corporativo(string email, bool expectedResult)
        {
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 1);

            bool isEmailCorporate = company.IsEmailCorporate(email);

            isEmailCorporate.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(5, 1, 6)]
        [InlineData(5, -5, 0)]
        public void Alterando_o_numero_de_empregados(int initial, int delta, int expectedResult)
        {
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: initial);

            company.ChangeNumberOfEmployees(delta);
            
            company.NumberOfEmployees.Should().Be(expectedResult);
        }

        [Fact]
        public void Alterando_o_numero_de_empregados_para_um_valor_negativo()
        {
            Company company = new Company(domain: "mycorp.com", numberOfEmployees: 5);

            Assert.Throws<InvalidOperationException>(() => company.ChangeNumberOfEmployees(-6));
        }
    }
}