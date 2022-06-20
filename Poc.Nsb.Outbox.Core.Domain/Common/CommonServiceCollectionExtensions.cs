using Microsoft.Extensions.DependencyInjection;

namespace Poc.Nsb.Outbox.Core.Domain.Common;

internal static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddDomainCommon(this IServiceCollection services)
    {
        return services;
    }
}
