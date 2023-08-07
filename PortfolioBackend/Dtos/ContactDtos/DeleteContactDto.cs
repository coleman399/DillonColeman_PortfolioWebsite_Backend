using System.ComponentModel.DataAnnotations;

namespace PortfolioBackend.Dtos.ContactDtos
{
    public class DeleteContactDto
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
    }
}
