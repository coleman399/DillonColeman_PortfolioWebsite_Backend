using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Role { get; set; } = "User";
        [Timestamp]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Timestamp]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User() { }

        public User(string userName, string passwordHash, string email, string role)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
        }

        public override string? ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(UserName)}: {UserName}, {nameof(Email)}: {Email}, {nameof(Role)}: {Role}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}
