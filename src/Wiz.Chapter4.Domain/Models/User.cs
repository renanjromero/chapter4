namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public UserType Type { get; set; }
    }
}