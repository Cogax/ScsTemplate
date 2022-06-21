using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Core.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDomain(this IServiceCollection services)
    {
        services.AddDomainCommon();

        return services;
    }
}

