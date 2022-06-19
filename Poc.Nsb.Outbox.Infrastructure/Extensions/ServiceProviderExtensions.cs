using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Poc.Nsb.Outbox.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static void Migrate<T>(this IServiceProvider serviceProvider)
        where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}
