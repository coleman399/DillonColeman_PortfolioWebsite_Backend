namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class UserServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string? Message { get; set; } = string.Empty;
    }
}
