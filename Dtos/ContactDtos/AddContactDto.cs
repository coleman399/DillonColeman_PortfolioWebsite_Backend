using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.ContactDtos
{
    public class AddContactDto
    {
        public string? Name { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Message { get; set; }
        [JsonIgnore, Timestamp]
        public DateTime UpdatedAt { get; set; }
    }
}
