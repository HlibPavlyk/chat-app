using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure.Extensions;

public static class ConnectionExtensions
{
    public static void AddDbConnection(this IServiceCollection service, IConfiguration configuration)
    {
        var serviceCollection = service.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("_"));
    }
}