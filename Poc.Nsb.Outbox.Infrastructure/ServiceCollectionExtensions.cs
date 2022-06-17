﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Poc.Nsb.Outbox.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PocDbContext>(optionsAction => optionsAction
            .UseSqlServer(config["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600)));

        return services;
    }
}