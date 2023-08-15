// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Azure.Identity;
using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Services;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;

namespace ManagedApplicationScheduler.AdminSite;

/// <summary>
/// Startup.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <value>
    /// The configuration.
    /// </value>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        var config = new ManagedAppClientConfiguration()
        {
            AdAuthenticationEndPoint = this.Configuration["AdAuthenticationEndPoint"],
            ClientId = this.Configuration["ClientId"] ?? Guid.Empty.ToString(),
            ClientSecret = this.Configuration["ClientSecret"] ?? String.Empty,
            GrantType = this.Configuration["GrantType"],
            SignedOutRedirectUri = this.Configuration["SignedOutRedirectUri"],
            TenantId = this.Configuration["TenantId"] ?? Guid.Empty.ToString(),
            DataBaseName = this.Configuration["CosmoDatabase"],
            PC_TenantId = this.Configuration["PC_TenantId"],
            PC_ClientID = this.Configuration["PC_ClientId"],
            PC_ClientSecret = this.Configuration["PC_ClientSecret"],
            PC_Scope = this.Configuration["PC_Scope"],
            Scope = this.Configuration["Scope"],
            Signature = this.Configuration["Signature"]
            
        };
        if (!config.AdAuthenticationEndPoint.EndsWith("/",StringComparison.OrdinalIgnoreCase))
            config.AdAuthenticationEndPoint = config.AdAuthenticationEndPoint + "/";

        var knownUsers = new KnownUsersModel()
        {
            KnownUsers = this.Configuration["KnownUsers"],
        };
        var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);


        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options =>
            {
                //options.Authority = $"{config.AdAuthenticationEndPoint}{config.TenantId}";
                options.Authority = $"{config.AdAuthenticationEndPoint}{config.TenantId}";
                options.ClientId = config.ClientId;
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/Home/Index";
                options.SignedOutRedirectUri = config.SignedOutRedirectUri;
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.NameClaimType = ClaimConstants.CLAIM_NAME; //This does not seem to take effect on User.Identity. See Note in CustomClaimsTransformation.cs
               options.TokenValidationParameters.ValidateIssuer = false;
            })
            .AddCookie();

        services
            .AddTransient<IClaimsTransformation, CustomClaimsTransformation>()
            .AddScoped<ExceptionHandlerAttribute>()
            .AddScoped<RequestLoggerActionFilter>()
        ;



        services
            .AddSingleton<ManagedAppClientConfiguration>(config)
            .AddSingleton<KnownUsersModel>(knownUsers);

        services
         .AddDbContext<CosmosDbContext>(options => options.UseCosmos(this.Configuration.GetConnectionString("DefaultConnection"), config.DataBaseName));
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddMvc(option => option.EnableEndpointRouting = false);


        services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });
        InitializeRepositoryServices(services);
    }

    /// <summary>
    /// Configures the specified application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The env.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
    }

    private static void InitializeRepositoryServices(IServiceCollection services)
    {
        services.AddScoped<KnownUserAttribute>();
        services.AddScoped<ISchedulerTasksRepository, SchedulerTasksRepository>();
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<IUsageResultRepository, UsageResultRepository>();
        services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
        services.AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>();
    }
}