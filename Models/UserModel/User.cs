using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public required string UserName { get; set; } = string.Empty;
        [Required]
        public required string PasswordHash { get; set; } = string.Empty;
        [Required, EmailAddress]
        public required string Email { get; set; } = string.Empty;
        [Required]
        public required string Role { get; set; } = Roles.User.ToString();
        [Required]
        public required string AccessToken { get; set; } = string.Empty;
        public RefreshToken? RefreshToken { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.MinValue;

        public User() { }

        public User(string userName, string passwordHash, string email, string role, RefreshToken refreshToken)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
            RefreshToken = refreshToken;
        }

        public override string? ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(UserName)}: {UserName}, {nameof(Email)}: {Email}, {nameof(Role)}: {Role}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}
