using System.Text.Json.Serialization;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class ConfigureDbContext
{
    public static void ConfigureMsSql(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MSSqlDbSettings");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    }
}