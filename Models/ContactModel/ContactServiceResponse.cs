namespace PortfolioWebsite_Backend.Models.ContactModel
{
    public class ContactServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string? Message { get; set; } = string.Empty;
    }
}
