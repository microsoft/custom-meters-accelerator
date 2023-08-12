// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

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
    /// Home Controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [Authorize]
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class HomeController : BaseController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HomeController> logger;

#pragma warning disable CS0169 // The field 'HomeController.loggerFactory' is never used
        private readonly ILoggerFactory loggerFactory;
#pragma warning restore CS0169 // The field 'HomeController.loggerFactory' is never used

        private SchedulerService schedulerService;
        private SubscriptionService subscriptionService;
        private ApplicationLogService applicationLogService;
#pragma warning disable CS0169 // The field 'HomeController.managedApiClientConfiguration' is never used
        private ManagedAppClientConfiguration managedApiClientConfiguration;
#pragma warning restore CS0169 // The field 'HomeController.managedApiClientConfiguration' is never used


        public HomeController(ILogger<HomeController> logger, ISubscriptionsRepository subscriptionsRepository, ISchedulerTasksRepository schedulerTasksRepository, IApplicationLogRepository applicationLogRepository)
        {
            this.logger = logger;
            this.subscriptionService = new SubscriptionService(subscriptionsRepository);
            this.schedulerService = new SchedulerService(schedulerTasksRepository,null,null);
            this.applicationLogService = new ApplicationLogService(applicationLogRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns> The <see cref="IActionResult" />.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Home Controller / Index ");
            try
            {
                // var userId = this.userService.AddUser(this.GetCurrentUserDetail());

                return this.View();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptionses this instance.
        /// </summary>
        /// <returns> The <see cref="IActionResult" />.</returns>
        [Authorize]
        public IActionResult Subscriptions()
        {
            this.logger.LogInformation("Home Controller / Subscriptions ");
            SummarySubscriptionViewModel summarySubscription = new SummarySubscriptionViewModel();

            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    summarySubscription.Subscriptions = this.subscriptionService.GetSubscriptionsView();
                    summarySubscription.IsSuccess = true;
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                summarySubscription.ErrorMessage = ex.Message;

            }

            return this.View(summarySubscription);
        }

        /// <summary>
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// Subscription log detail.
        /// </returns>
        [Authorize]
        public IActionResult SubscriptionDetails(string id)
        {
            var subscriptionDetail = new SubscriptionModel();

            this.logger.LogInformation("Home Controller / SubscriptionLogDetail : subscriptionId: {0}", JsonSerializer.Serialize(id));

            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    subscriptionDetail = this.subscriptionService.GetSubscriptionByID(id);
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

        /// <summary>
        /// The Error.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />.
        /// </returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionDetail = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return this.View(exceptionDetail?.Error);
        }
        [Authorize]
        public ActionResult NewSubscription()
        {
            var subscriptionDetail = new SubscriptionModel();

            this.logger.LogInformation("Home Controller / Add New Subscription");
            
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

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
        [Authorize]
        // POST: Subscription/Create
        [HttpPost]
        public ActionResult NewSubscriptionAction(SubscriptionModel subscription)
        {

            var subscriptionDetail = new SubscriptionModel();

            this.logger.LogInformation("Home Controller / Add New Subscription");
            this.applicationLogService.AddApplicationLog($"Start Saving new Subscription to Db: {JsonSerializer.Serialize(subscription)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        subscription.SubscriptionStatus = "Subscribed";
                        subscription.ProvisionState = "Succeeded";
                        subscription.id = subscription.id.Replace("/", "|");

                        this.subscriptionService.SaveSubscription(subscription);
                        this.applicationLogService.AddApplicationLog($"Completed Saving new Subscription Id: {HttpUtility.HtmlEncode(subscription.id)}");
                        return RedirectToAction("Subscriptions");
                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during Saving new Subscription with Id {HttpUtility.HtmlEncode(subscription.id)} to Db: {ex.Message}");
                subscriptionDetail.ErrorMessage = ex.Message;

            }


            return this.View(subscriptionDetail);
        }
        [Authorize]
        public ActionResult EditSubscription(string subscriptionId)
        {

            this.logger.LogInformation("Home Controller / Edit Subscription");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        SubscriptionModel subscription = this.subscriptionService.GetSubscriptionByID(subscriptionId);
                        return View(subscription);
                    }

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
            return this.RedirectToAction("Subscriptions");
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditSubscriptionAction(SubscriptionModel subscription)
        {
            this.logger.LogInformation("Home Controller / Edit Subscription Action");
            this.applicationLogService.AddApplicationLog($"Start Saving Subscription to Db: {JsonSerializer.Serialize(subscription)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        this.subscriptionService.UpdateSubscription(subscription);
                        this.applicationLogService.AddApplicationLog($"Completed Saving  Subscription Id: {HttpUtility.HtmlEncode(subscription.id)}");
                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during Saving  Subscription with Id {HttpUtility.HtmlEncode(subscription.id)} to Db: {ex.Message}");

            }
            return this.RedirectToAction("Subscriptions");
        }
        [Authorize]
        public ActionResult DeleteSubscription(string subscriptionId)
        {
            this.logger.LogInformation("Home Controller / Delete Subscription Action");
            this.applicationLogService.AddApplicationLog($"Start Deleting Subscription from Db: {HttpUtility.HtmlEncode(subscriptionId)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        var schedulerTasks = this.schedulerService.GetSchedulersTasksBySubscription(subscriptionId.Replace("|", "/"));
                        if (schedulerTasks.Count > 0)
                        {
                            this.subscriptionService.UpdateSubscriptionStatus(subscriptionId, "Unsubscribed");
                        }
                        else
                        {

                            this.subscriptionService.DeleteSubscription(subscriptionId);
                            this.applicationLogService.AddApplicationLog($"Completed Deleting Subscription from Db: {HttpUtility.HtmlEncode(subscriptionId)}");
                        }

                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during deleting  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");

            }
            return this.RedirectToAction(nameof(this.Subscriptions));

        }
        [Authorize]
        public ActionResult Unsubscribe(string subscriptionId)
        {
            this.logger.LogInformation("Home Controller / Unsubscribe Subscription Action");
            this.applicationLogService.AddApplicationLog($"Start Unsubscribe Subscription : {HttpUtility.HtmlEncode(subscriptionId)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        this.subscriptionService.UpdateSubscriptionStatus(subscriptionId, "Unsubscribed");
                        this.applicationLogService.AddApplicationLog($"Completed Unsubscribe Subscription : {HttpUtility.HtmlEncode(subscriptionId)}");
                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during unsubscribe  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");

            }
            return this.RedirectToAction(nameof(this.Subscriptions));
        }
        [Authorize]
        public ActionResult Subscribe(string subscriptionId)
        {
            this.logger.LogInformation("Home Controller / Subscribe Subscription Action");
            this.applicationLogService.AddApplicationLog($"Start subscribe Subscription : {HttpUtility.HtmlEncode(subscriptionId)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        this.subscriptionService.UpdateSubscriptionStatus(subscriptionId, "Subscribed");
                        this.applicationLogService.AddApplicationLog($"Completed subscribe Subscription : {HttpUtility.HtmlEncode(subscriptionId)}");

                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during subscribe  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");

            }
            return this.RedirectToAction(nameof(this.Subscriptions));
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

    }
}