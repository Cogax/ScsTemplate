using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

public static class UnitOfWorkServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TransactionScopeUnitOfWork>();
        services.AddScoped<DefaultUnitOfWork>();
        services.AddScoped<DefaultUnitOfWork>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}
