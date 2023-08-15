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
        private readonly SubscriptionService subscriptionService;

        private readonly SchedulerService schedulerService;

        private readonly UsageResultService usageResultService;

        private readonly ApplicationLogService applicationLogService;

        private readonly ILogger<SchedulerController> logger;

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
        [HttpGet]
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                throw;
            }
            return this.View(data);

        }
        [HttpGet]
        public IActionResult NewScheduler(string id)
        {

            this.logger.LogInformation("New Scheduler Controller");
            if (this.User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";
                try
                {
                    SchedulerUsageViewModel schedulerUsageViewModel = new ();

                    var allActiveMeteredSubscriptions = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();

                    // Create Frequency Dropdown list
                    List<SelectListItem> SchedulerFrequencyList = new()
                    {
                        new SelectListItem()
                        {
                            Text = "OneTime",
                            Value = SchedulerFrequencyEnum.OneTime.ToString(),
                        }
                    };


                    // Create Subscription Dropdown list
                    List<SelectListItem> SubscriptionList = new ();
                    List<SelectListItem> DimensionsList = new();
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
                    this.logger.LogError("{Message}",ex.Message);
                    throw;
                }
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }


        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddNewScheduledTrigger(SchedulerUsageViewModel schedulerUsageViewModel)
        {
            if (schedulerUsageViewModel == null)
            {
                throw new ArgumentNullException(nameof(schedulerUsageViewModel));
            }
            try
            {
                this.applicationLogService.AddApplicationLog($"Start Adding new Task : {JsonSerializer.Serialize(schedulerUsageViewModel)}");
                var sub = this.subscriptionService.GetSubscriptionByID(schedulerUsageViewModel.SelectedSubscription);
                ScheduledTasksModel schedulerManagement = new()
                {
                    id = Guid.NewGuid().ToString(),
                    Frequency = schedulerUsageViewModel.SelectedSchedulerFrequency,
                    ScheduledTaskName = schedulerUsageViewModel.SchedulerName,
                    ResourceUri = schedulerUsageViewModel.SelectedSubscription.Replace("|", "/", StringComparison.OrdinalIgnoreCase),
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
                this.logger.LogError("{Message}", ex.Message);
                this.applicationLogService.AddApplicationLog($"Error during Saving Task with Name {HttpUtility.HtmlEncode(schedulerUsageViewModel.SchedulerName)} to Db: {ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param id="schedule Id">The plan gu identifier.</param>
        /// <returns>
        /// return All subscription.
        [HttpGet]
        public IActionResult DeleteSchedulerItem(string id)
        {
            this.logger.LogInformation("Scheduler Controller / Remove Schedule Item Details:  Id {Id}", HttpUtility.HtmlEncode(id));
            this.applicationLogService.AddApplicationLog($"Start Deleting Task with Id : {HttpUtility.HtmlEncode(id)}");
            try
            {
                this.schedulerService.DeleteScheduler(id);
                this.applicationLogService.AddApplicationLog($"Completed Deleting Task with Id : {HttpUtility.HtmlEncode(id)}");
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.logger.LogError("{Message}", ex.Message);
                this.applicationLogService.AddApplicationLog($"Error during Saving Task with ID {HttpUtility.HtmlEncode(id)} to Db: {ex.Message}");
                throw;
            }

        }
        [HttpGet]
        public IActionResult SchedulerLogDetail(string id)
        {
            var task = new ScheduledTasksModel();
            this.logger.LogInformation("Scheduler Controller / SubscriptionLogDetail : subscriptionId: {Id}", JsonSerializer.Serialize(id));
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                throw;
            }
            return this.View(task);
        }
        [HttpGet]
        public IActionResult SubscriptionMeteredDetail(string subscriptionId)
        {

            if(subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            var subscriptionDetail = new SubscriptionViewModel();

            this.logger.LogInformation("Scheduler Controller / SubscriptionLogDetail : subscriptionId: {Id}", JsonSerializer.Serialize(subscriptionId));

            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    subscriptionDetail = this.subscriptionService.GetSubscriptionsViewById(subscriptionId);
                    subscriptionDetail.meteringUsageResultModels = this.usageResultService.GetUsageBySubscription(subscriptionId.Replace("|", "/",StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                subscriptionDetail.ErrorMessage = ex.Message;
                throw;
            }
            return this.View(subscriptionDetail);

        }
        [HttpGet]
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
                    List<SelectListItem> selectedList = new();
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