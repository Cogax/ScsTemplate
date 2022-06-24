### Adding a new migration:

1. `git commit` so that we can revert our changes quickly
1. Package Manager Console: `Add-Migration <migration-name> -Context WriteModelDbContext -OutputDir Adapters/Persistence/Migrations -Project Cogax.SelfContainedSystem.Template.Infrastructure -StartupProject Cogax.SelfContainedSystem.Template.Web`
1. Start `Web` project so that database gets migrated (requires `docker-compose up` executed beforehand)
1. Package Manager Console: `Scaffold-DbContext 'server=localhost;database=PocDb;user=sa;password=Top-Secret;' Microsoft.EntityFrameworkCore.SqlServer -DataAnnotations -Context ReadModelDbContext -ContextDir Adapters/Persistence/DbContexts -OutputDir Adapters/Persistence/Generated -Project Cogax.SelfContainedSystem.Template.Infrastructure -StartupProject Cogax.SelfContainedSystem.Template.Web -NoPluralize -Force`
