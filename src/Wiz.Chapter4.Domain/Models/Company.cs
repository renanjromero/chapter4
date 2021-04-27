namespace Wiz.Chapter4.Domain.Models
{
    public class Company
    {
        public Company(string domain, int numberOfEmployees)
        {
            Domain = domain;
            NumberOfEmployees = numberOfEmployees;
        }

        public string Domain { get; set; }

        public int NumberOfEmployees { get; set; }
    }
}