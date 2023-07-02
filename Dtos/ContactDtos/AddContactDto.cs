namespace DillonColeman_PortfolioWebsite.Dtos.ContactDtos
{
    public class AddContactDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Message { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
