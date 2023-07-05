using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        [Timestamp]
        public DateTime CreatedAt { get; set; }
        [Timestamp]
        public DateTime UpdatedAt { get; set; }
    }
}
