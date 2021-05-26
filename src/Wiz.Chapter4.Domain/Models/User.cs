using Wiz.Chapter4.Domain.Enums;

namespace Wiz.Chapter4.Domain.Models
{
    public class User
    {
        public User(int id, string email, UserType type)
        {
            Id = id;
            Email = email;
            Type = type;
        }

        public int Id { get; set; }

        public string Email { get; set; }

        public UserType Type { get; set; }
    }
}   