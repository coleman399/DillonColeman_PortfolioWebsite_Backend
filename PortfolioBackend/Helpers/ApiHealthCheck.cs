using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PortfolioBackend.Helpers
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly UserContext _userContext;
        private readonly IConfiguration _configuration;

        public ApiHealthCheck(UserContext userContext, IConfiguration configuration)
        {
            _userContext = userContext;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                //SuperUser Check
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
