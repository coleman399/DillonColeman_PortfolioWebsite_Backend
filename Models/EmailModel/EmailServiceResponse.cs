namespace PortfolioWebsite_Backend.Models.EmailModel
{
    public class EmailServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string? Message { get; set; } = string.Empty;
    }
}
