using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Web;

namespace ManagedApplicationScheduler.AdminSite.Controllers
{
    /// <summary>
    /// ApplicationConfig Controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class ApplicationConfigController : BaseController
    {
        private readonly ILogger<ApplicationConfigController> logger;
        private readonly ApplicationConfigurationService appConfigService;
        private ApplicationLogService applicationLogService;
        /// <summary>
        /// Move to a new controller?
        /// </summary>


        public ApplicationConfigController(
            IApplicationConfigurationRepository applicationConfigurationRepository,  ILogger<ApplicationConfigController> logger, IApplicationLogRepository applicationLogRepository)
        {
            this.appConfigService = new ApplicationConfigurationService(applicationConfigurationRepository);
            this.applicationLogService = new ApplicationLogService(applicationLogRepository);
            this.logger = logger;
        }

        /// <summary>
        /// Main action for Application Config Page
        /// </summary>
        /// <returns>return All Application Config.</returns>
        [ServiceFilter(typeof(ExceptionHandlerAttribute))]
        public IActionResult Index()
        {
            var getAllAppConfigData = this.appConfigService.GetAllConfig();
            return this.View(getAllAppConfigData);
        }


        /// <summary>
        /// Get the apllication config item by Id.
        /// </summary>
        /// <param name="Id">The app config Id.</param>
        /// <returns>
        /// return an Application Config item.
        /// </returns>
        public IActionResult ApplicationConfigDetails(string id)
        {
            var applicationConfiguration = this.appConfigService.GetById(id);
            return this.PartialView(applicationConfiguration);
        }


        /// <summary>
        /// Saves the app config item changes.
        /// </summary>
        /// <param name="appConfig">The app config item.</param>
        /// <returns>
        /// return the changed app config item.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplicationConfigDetails(ApplicationConfigurationModel appConfig)
        {
            applicationLogService.AddApplicationLog($"Saving configuration {JsonSerializer.Serialize(appConfig)}");
            
            this.appConfigService.UpdateApplicationConfig(appConfig);
            
            applicationLogService.AddApplicationLog($"Completed Saving configuration ");

            this.ModelState.Clear();
            return new OkResult();
        }

    }
}