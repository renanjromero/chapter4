namespace Wiz.Chapter4.Domain.Models
{
    public class Company
    {
        public Company(int id, string domainName, int numberOfEmployees)
        {
            this.Id = id;
            this.DomainName = domainName;
            this.NumberOfEmployees = numberOfEmployees;
        }

        public int Id { get; set; }
        public string DomainName { get; set; }
        public int NumberOfEmployees { get; set; }
    }
}