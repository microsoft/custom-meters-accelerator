using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace ManagedApplicationScheduler.AdminSite.Controllers
{

    [ServiceFilter(typeof(KnownUserAttribute))]
    public class ApplicationLogController : BaseController
    {
        private readonly ILogger<ApplicationLogController> logger;

        private readonly ApplicationLogService appLogService;


        private readonly IApplicationLogRepository appLogRepository;

        public ApplicationLogController(IApplicationLogRepository applicationLogRepository, ILogger<ApplicationLogController> logger)
        {
            this.appLogRepository = applicationLogRepository;
            this.logger = logger;
            appLogService = new ApplicationLogService(this.appLogRepository);
        }
        [HttpGet]
        public IActionResult Index()
        {
            this.logger.LogInformation("Application Log Controller / Index");
            try
            {
                List<ApplicationLogModel> getAllAppLogData = this.appLogService.GetAllLogs().OrderByDescending(appLog => appLog.ActionTime).ToList();
                getAllAppLogData.ForEach(s => s.LogDetail = Regex.Replace(s.LogDetail, "&quot;", "\""));
                return this.View(getAllAppLogData);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{Message} :: {InnerException}", ex.Message, ex.InnerException);
                throw;
            }
        }

    }
}

