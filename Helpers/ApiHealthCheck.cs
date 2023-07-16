using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;

namespace PortfolioWebsite_Backend.Helpers
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

        private static string GetRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];
            return new string(stringChars);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Logging Check
                const string TEMP_LOG_PATH = @"./Helpers/temp/log.txt";
                File.WriteAllText(TEMP_LOG_PATH, string.Empty);
                var logFile = new FileInfo(TEMP_LOG_PATH);
                if (!logFile.Exists)
                    return HealthCheckResult.Unhealthy("Logging file does not exist");
                var searchString = GetRandomString(15);
                var logger = new LoggerConfiguration().WriteTo.File(logFile.FullName).CreateLogger();
                logger.Information(searchString);
                logger.Dispose();
                Log.Logger.Information($"Logging file exists - {searchString}");
                var fs = new FileStream(TEMP_LOG_PATH, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                var logs = new List<string>();
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync(cancellationToken)) != null)
                    {
                        logs.Add(line);
                    }
                }
                foreach (var log in logs)
                {
                    string logString = log.ToString();

                    if (!log.Contains(searchString))
                        return HealthCheckResult.Unhealthy("Logging not writing to log.");
                }
                using (fs = new(TEMP_LOG_PATH, FileMode.Truncate)) { }

                // SuperUser Check
                var dbUsers = await _userContext.Users.ToListAsync(cancellationToken: cancellationToken);
                if (_userContext.Users.FirstOrDefault(u => u.Email == _configuration["SuperUser:Email"]) == null)
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
