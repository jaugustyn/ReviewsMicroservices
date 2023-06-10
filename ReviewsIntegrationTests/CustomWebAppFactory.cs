using System.Data.Common;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ReviewsIntegrationTests;

public class CustomWebAppFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
{
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddJsonFile(configPath);
        });
            
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