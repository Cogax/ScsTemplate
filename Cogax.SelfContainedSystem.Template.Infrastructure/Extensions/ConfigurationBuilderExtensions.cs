using Microsoft.Extensions.Configuration;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder ConfigureDefaultConfig(this IConfigurationBuilder configBuilder, string environment, string contentRootPath, Action<IConfigurationBuilder>? configAction = null)
    {
        configBuilder.AddEnvironmentVariables(prefix: "ASPNETCORE_");

        configBuilder.AddJsonFile(Path.Combine(Path.Combine(contentRootPath, "..", "Cogax.SelfContainedSystem.Template.Infrastructure"), "appsettings.infrastructure.json"), optional: true, reloadOnChange: true); // When running using ef migrations or dotnet run
        configBuilder.AddJsonFile("appsettings.infrastructure.json", optional: true, reloadOnChange: true); // When app is published
        configBuilder.AddJsonFile($"appsettings.infrastructure.{environment}.json", optional: true, reloadOnChange: true);
        configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        configBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        configBuilder.AddEnvironmentVariables();

        configAction?.Invoke(configBuilder); // Inject additional configuration (tests)
        return configBuilder;
    }
}
