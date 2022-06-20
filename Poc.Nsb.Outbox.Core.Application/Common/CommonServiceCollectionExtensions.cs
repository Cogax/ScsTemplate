using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Poc.Nsb.Outbox.Core.Application.Common.Consistency;
using Poc.Nsb.Outbox.Core.Domain;

namespace Poc.Nsb.Outbox.Core.Application.Common;

internal static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationCommon(this IServiceCollection services)
    {
        services.AddMediatR(
            typeof(ApplicationServiceCollectionExtensions),
            typeof(DomainServiceCollectionExtensions));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
