using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace ManagedApplicationScheduler.AdminSite.Controllers
{
    /// <summary>
    /// Scheduler Controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [Authorize]
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class SchedulerController : BaseController
    {

        /// <summary>
        /// the subscription service
        /// </summary>
        private SubscriptionService subscriptionService;

        private SchedulerService schedulerService;

        private UsageResultService usageResultService;

        private ApplicationLogService applicationLogService;

        private readonly ILogger<SchedulerController> logger;

#pragma warning disable CS0169 // The field 'SchedulerController.loggerFactory' is never used
        private readonly ILoggerFactory loggerFactory;
#pragma warning restore CS0169 // The field 'SchedulerController.loggerFactory' is never used





        /// <summary>

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansController" /> class.
        /// </summary>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="offerAttributeRepository">The offer attribute repository.</param>
        /// <param name="offerRepository">The offer repository.</param>
        /// <param name="logger">The logger.</param>
        public SchedulerController(ILogger<SchedulerController> logger, ISubscriptionsRepository subscriptionsRepository
            , ISchedulerTasksRepository schedulerTasksRepository
            , IUsageResultRepository usageResultRepository, IApplicationLogRepository applicationLogRepository)

        {
            this.schedulerService = new SchedulerService(schedulerTasksRepository,null,null);
            this.subscriptionService = new SubscriptionService(subscriptionsRepository);
            this.usageResultService = new UsageResultService(usageResultRepository);
            this.applicationLogService = new ApplicationLogService(applicationLogRepository);
            this.logger = logger;

        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription.</returns>
        public IActionResult Index()
        {
            var data = new List<ScheduledTasksModel>();
            this.logger.LogInformation("Scheduler Controller / Get all data");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    data = this.schedulerService.GetAllSchedulersTasks();
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
            }
            return this.View(data);

        }

        public IActionResult NewScheduler(string id)
        {

            this.logger.LogInformation("New Scheduler Controller");
            if (this.User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";
                try
                {
                    SchedulerUsageViewModel schedulerUsageViewModel = new SchedulerUsageViewModel();

                    var allActiveMeteredSubscriptions = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();

                    // Create Frequency Dropdown list
                    List<SelectListItem> SchedulerFrequencyList = new List<SelectListItem>();
                    SchedulerFrequencyList.Add(new SelectListItem()
                    {
                        Text = "OneTime",
                        Value = SchedulerFrequencyEnum.OneTime.ToString(),
                    });


                    // Create Subscription Dropdown list
                    List<SelectListItem> SubscriptionList = new List<SelectListItem>();
                    List<SelectListItem> DimensionsList = new List<SelectListItem>();
                    foreach (var item in allActiveMeteredSubscriptions)
                    {
                        var sub = item.id.Split("|");
                        SubscriptionList.Add(new SelectListItem()
                        {
                            Text = sub[2] + "|" + sub[8],
                            Value = item.id.ToString(),
                        });

                        if (item.id == id)
                        {
                            var dimlist = item.Dimension.Split("|");
                            foreach (var dim in dimlist)
                            {
                                DimensionsList.Add(new SelectListItem()
                                {
                                    Text = dim,
                                    Value = dim
                                });
                            }
                        }

                    }
                    // Create Plan Dropdown list
                    schedulerUsageViewModel.DimensionsList = new SelectList(DimensionsList, "Value", "Text");
                    schedulerUsageViewModel.SubscriptionList = new SelectList(SubscriptionList, "Value", "Text");
                    schedulerUsageViewModel.SchedulerFrequencyList = new SelectList(SchedulerFrequencyList, "Value", "Text");
                    schedulerUsageViewModel.SelectedSubscription = id;


                    return this.View(schedulerUsageViewModel);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, ex.Message);
                    return this.View("Error", ex);
                }
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }


        }



        public IActionResult AddNewScheduledTrigger(SchedulerUsageViewModel schedulerUsageViewModel)
        {

            try
            {
                this.applicationLogService.AddApplicationLog($"Start Adding new Task : {JsonSerializer.Serialize(schedulerUsageViewModel)}");
                var sub = this.subscriptionService.GetSubscriptionByID(schedulerUsageViewModel.SelectedSubscription);
                ScheduledTasksModel schedulerManagement = new ScheduledTasksModel()
                {
                    id = Guid.NewGuid().ToString(),
                    Frequency = schedulerUsageViewModel.SelectedSchedulerFrequency,
                    ScheduledTaskName = schedulerUsageViewModel.SchedulerName,
                    ResourceUri = schedulerUsageViewModel.SelectedSubscription.Replace("|", "/"),
                    //PlanId = selectedDimension.PlanId,
                    Dimension = schedulerUsageViewModel.SelectedDimension,
                    Quantity = Convert.ToDouble(schedulerUsageViewModel.Quantity),
                    StartDate = schedulerUsageViewModel.FirstRunDate.AddHours(schedulerUsageViewModel.TimezoneOffset),
                    Status = "Scheduled",
                    PlanId = sub.PlanId
                };
                this.schedulerService.SaveScheduler(schedulerManagement);
                this.applicationLogService.AddApplicationLog($"Completed Adding new Task : {HttpUtility.HtmlEncode(schedulerUsageViewModel.SchedulerName)}");
                return this.RedirectToAction(nameof(this.Index));

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                this.applicationLogService.AddApplicationLog($"Error during Saving Task with Name {HttpUtility.HtmlEncode(schedulerUsageViewModel.SchedulerName)} to Db: {ex.Message}");
                return this.View("Error", ex);
            }

#pragma warning disable CS0162 // Unreachable code detected
            return this.View();
#pragma warning restore CS0162 // Unreachable code detected
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param id="schedule Id">The plan gu identifier.</param>
        /// <returns>
        /// return All subscription.

        public IActionResult DeleteSchedulerItem(string id)
        {
            this.logger.LogInformation($"Scheduler Controller / Remove Schedule Item Details:  Id {HttpUtility.HtmlEncode(id)}");
            this.applicationLogService.AddApplicationLog($"Start Deleting Task with Id : {HttpUtility.HtmlEncode(id)}");
            try
            {
                this.schedulerService.DeleteScheduler(id);
                this.applicationLogService.AddApplicationLog($"Completed Deleting Task with Id : {HttpUtility.HtmlEncode(id)}");
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                this.applicationLogService.AddApplicationLog($"Error during Saving Task with ID {HttpUtility.HtmlEncode(id)} to Db: {ex.Message}");

                return this.PartialView("Error", ex);
            }

#pragma warning disable CS0162 // Unreachable code detected
            return this.View();
#pragma warning restore CS0162 // Unreachable code detected
        }

        public IActionResult SchedulerLogDetail(string id)
        {
            var task = new ScheduledTasksModel();
            this.logger.LogInformation("Scheduler Controller / SubscriptionLogDetail : subscriptionId: {0}", JsonSerializer.Serialize(id));
            try
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    task = this.schedulerService.GetSchedulerByID(id);
                    task.MeteredUsageResult = this.usageResultService.GetUsageByTaskName(task.ScheduledTaskName, task.ResourceUri);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
            }
            return this.View(task);
        }

        public IActionResult SubscriptionMeteredDetail(string subscriptionId)
        {
            var subscriptionDetail = new SubscriptionViewModel();

            this.logger.LogInformation("Scheduler Controller / SubscriptionLogDetail : subscriptionId: {0}", JsonSerializer.Serialize(subscriptionId));

            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    subscriptionDetail = this.subscriptionService.GetSubscriptionsViewById(subscriptionId);
                    subscriptionDetail.meteringUsageResultModels = this.usageResultService.GetUsageBySubscription(subscriptionId.Replace("|", "/"));
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                subscriptionDetail.ErrorMessage = ex.Message;
            }
            return this.View(subscriptionDetail);

        }

        public IActionResult GetSubscriptionData(string id)
        {
            var allSubscriptionDetails = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();
            var selectSubscription = allSubscriptionDetails.Where(s => s.id == id).FirstOrDefault();
            if (selectSubscription != null)
            {
                // Create Dimension Dropdown list
                var getAllDimensions = selectSubscription.Dimension.Split('|');
                if (getAllDimensions != null)
                {
                    List<SelectListItem> selectedList = new List<SelectListItem>();
                    foreach (var item in getAllDimensions)
                    {
                        selectedList.Add(new SelectListItem()
                        {
                            Text = item,
                            Value = item,
                        });
                    }

                    return Json(selectedList);

                }
                return this.PartialView("Error", "Can not find any metered dimension related to selected plan");

            }
            return this.PartialView("Error", "Subscription is Invalid");
        }

    }
}