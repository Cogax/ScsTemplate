using Microsoft.Extensions.DependencyInjection;

using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDomain(this IServiceCollection services)
    {
        services.AddDomainCommon();

        return services;
    }
}

