using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite_Backend.Dtos.ContactDtos
{
    public class DeleteContactDto
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
    }
}
