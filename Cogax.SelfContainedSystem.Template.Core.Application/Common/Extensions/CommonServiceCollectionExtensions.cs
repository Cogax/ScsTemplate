using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Domain;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Extensions;

internal static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationCommon(this IServiceCollection services)
    {
        services.AddMediatR(
            typeof(ApplicationServiceCollectionExtensions),
            typeof(DomainServiceCollectionExtensions));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IUnitOfWorkFactory>().Create());
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddSingleton<IChaosMonkey, NullChaosMonkey>();

        return services;
    }
}
