using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WordAnalytics.Tests.Integration
{
    public class WordAnalyticsApiFactory : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string?> _configuration = new()
        {
            ["RateLimiting:PermitLimit"] = "10000",
            ["ConnectionStrings:WordAnalytics"] =
                "Host=localhost;Port=5432;Database=wordanalytics;Username=wordanalytics;Password=wordanalytics-local-only"
        };
        private Action<IServiceCollection>? _configureServices;

        public WordAnalyticsApiFactory WithConfiguration(string key, string? value)
        {
            _configuration[key] = value;
            return this;
        }

        public WordAnalyticsApiFactory WithServices(Action<IServiceCollection> configure)
        {
            _configureServices = configure;
            return this;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting(
                "ConnectionStrings:WordAnalytics",
                _configuration["ConnectionStrings:WordAnalytics"]);

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(_configuration);
            });

            if (_configureServices is not null)
            {
                builder.ConfigureTestServices(_configureServices);
            }
        }
    }
}
