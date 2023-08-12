using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Services;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Contracts;
using ManagedApplicationScheduler.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ManagedApplicationScheduler.MeteredTriggerJob;

class Program
{
    static void Main(string[] args)
    {

        Console.WriteLine($"MeteredExecutor Webjob Started at: {DateTime.Now}");

        IConfiguration configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
           // .AddJsonFile("appsettings.json")
            .Build();

        var config = new ManagedAppClientConfiguration()
        {
            AdAuthenticationEndPoint = configuration["AdAuthenticationEndPoint"],
            ClientId = configuration["ClientId"] ?? Guid.Empty.ToString(),
            ClientSecret = configuration["ClientSecret"] ?? String.Empty,
            GrantType = configuration["GrantType"],
            SignedOutRedirectUri = configuration["SignedOutRedirectUri"],
            TenantId = configuration["TenantId"] ?? Guid.Empty.ToString(),
            DataBaseName = configuration["CosmoDatabase"],
            PC_TenantId = configuration["PC_TenantId"],
            PC_ClientID = configuration["PC_ClientId"],
            PC_ClientSecret = configuration["PC_ClientSecret"],
            PC_Scope = configuration["PC_Scope"],
            Scope = configuration["Scope"],
            Marketplace_Uri = configuration["Marketplace_Uri"]

        };


        var services = new ServiceCollection()
            .AddDbContext<CosmosDbContext>(options => options.UseCosmos(configuration.GetConnectionString("DefaultConnection"), config.DataBaseName))
            .AddScoped<ISchedulerTasksRepository, SchedulerTasksRepository>()
            .AddScoped<ISubscriptionsRepository, SubscriptionsRepository>()
            .AddScoped<IUsageResultRepository, UsageResultRepository>()
            .AddScoped<IApplicationLogRepository, ApplicationLogRepository>()
            .AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>()
            .AddScoped<IEmailService, SMTPEmailService>()
            .AddSingleton<ManagedAppClientConfiguration>(config)
            .AddSingleton<Executor, Executor>()
            .BuildServiceProvider();

        services
            .GetService<Executor>()
            .ExecuteAsync();
        Console.WriteLine($"MeteredExecutor Webjob Ended at: {DateTime.Now}");

    }
}