using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReviewsIntegrationTests;

public class CustomWebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        builder.ConfigureAppConfiguration((context, conf) => { conf.AddJsonFile(configPath); });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestScheme";
                o.DefaultChallengeScheme = "TestScheme";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
        });
        builder.UseEnvironment("Development");

        base.ConfigureWebHost(builder);
    }
}


public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test user"),
            new Claim(ClaimTypes.NameIdentifier, "userId"),
            new Claim(ClaimTypes.Email, "user@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}