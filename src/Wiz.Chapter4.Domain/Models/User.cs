namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {

        public User(int id, int companyId, string email, UserType type)
        {
            this.Id = id;
            this.CompanyId = companyId;
            this.Email = email;
            this.Type = type;
        }
        
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
    }
}