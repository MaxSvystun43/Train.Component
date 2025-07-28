using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Train.Component.Management.Database;
using Train.Component.Management.Database.Configurations;
using Train.Component.Management.Migrations;
using Train.Component.Management.Service;

namespace Train.Component.Management.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void AddTrainComponents(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<IItemService, ItemService>();
        
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var databaseConfiguration = builder.Configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>()
                                    ?? throw new ApplicationException("Database configuration not found");

        builder.Services.AddOptions<DatabaseConfiguration>().Bind(builder.Configuration.GetSection("DatabaseConfiguration"));
        
        builder.Services.AddDbContext<ManagementDbContext>(option => option.UseNpgsql(databaseConfiguration.ConnectionString).UseSnakeCaseNamingConvention());

        builder.Services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(databaseConfiguration.ConnectionString)
                .ScanIn(typeof(Migration_Init).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .Configure<FluentMigratorLoggerOptions>(opt => {
                opt.ShowSql = true;
                opt.ShowElapsedTime = true;
            });
    }
    
}