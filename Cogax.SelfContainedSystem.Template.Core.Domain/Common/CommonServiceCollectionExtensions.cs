using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Common;

internal static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddDomainCommon(this IServiceCollection services)
    {
        return services;
    }
}
