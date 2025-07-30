using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Train.Component.Management.Database;
using Train.Component.Management.Database.Configurations;
using Train.Component.Management.Extensions;
using Train.Component.Management.Migrations;
using Train.Component.Management.Service;

var builder = WebApplication.CreateBuilder(args);

builder.AddTrainComponents();
builder.AddDatabase();

var app = builder.Build();

await RunMigrationsAsync(app);

// Helper method to run migrations
static async Task RunMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    
    try
    {
        logger.LogInformation("Checking for pending database migrations...");
        
        // Check if database connection is available
        await Task.Run(() =>
        {
            if (runner.HasMigrationsToApplyUp())
            {
                logger.LogInformation("Found pending migrations. Running database migrations...");
                runner.MigrateUp();
                logger.LogInformation("Database migrations completed successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No migrations needed.");
            }
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error running database migrations: {ErrorMessage}", ex.Message);

        throw new InvalidOperationException("Database migration failed. Application cannot start.", ex);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.AddEndpoints();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.Run();
