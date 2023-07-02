namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; } = string.Empty;
        public required string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User()
        {
        }

        public User(int id, string userName, string passwordHash, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public override string? ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(UserName)}: {UserName}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
        }
    }
}
