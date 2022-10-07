using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    private static async Task Main(string[] args)
    {

        var upOption = new Option("--up", "Migrate Up");
        upOption.Arity = ArgumentArity.ZeroOrOne;
        upOption.SetDefaultValue(false);
        var downOption = new Option("--down", "Rollback database to a version");
        downOption.Arity = ArgumentArity.ExactlyOne;
        downOption.SetDefaultValue(-1);

        var rootCommand = new RootCommand();
        rootCommand.Description = "Recipe Fluent Migrator Runner";
        rootCommand.AddOption(upOption);
        rootCommand.AddOption(downOption);
        rootCommand.Handler = CommandHandler.Create<bool, long>((up, down) =>
        {
            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                if (up)
                    UpdateDatabase(scope.ServiceProvider);

                else if (down > -1)
                    RollbackDatabase(scope.ServiceProvider, down);

            }
        });

        await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static IServiceProvider CreateServices()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();
        var connectionString = configuration["ConnectionString"];

        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add Postgres support to FluentMigrator
                .AddSqlServer()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        try
        {
            runner.MigrateUp();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

    private static void RollbackDatabase(IServiceProvider serviceProvider, long rollbackVersion)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        // Execute the migrations
        try
        {
            runner.MigrateDown(rollbackVersion);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}