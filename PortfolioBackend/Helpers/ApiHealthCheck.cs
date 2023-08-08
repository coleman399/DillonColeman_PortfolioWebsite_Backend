using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PortfolioBackend.Helpers
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly bool isDevelopment;
        private readonly UserContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApiHealthCheck(UserContext userContext, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userContext = userContext;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            isDevelopment = _webHostEnvironment.IsDevelopment();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                //SuperUser Check
                if (isDevelopment)
                    return HealthCheckResult.Healthy();
                var dbUsers = await _userContext.Users.ToListAsync(cancellationToken: cancellationToken);
                if (_userContext.Users.FirstOrDefault(u => u.Email == _configuration["SuperUserEmail"]) == null)
                    return HealthCheckResult.Unhealthy();

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy(exception.Message + " " + exception);
            }
        }
    }
}
