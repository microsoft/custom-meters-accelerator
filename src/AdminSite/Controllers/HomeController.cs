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

        private readonly SchedulerService schedulerService;
        private readonly SubscriptionService subscriptionService;
        private readonly ApplicationLogService applicationLogService;

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
        [HttpGet]
        public IActionResult Index()
        {
            this.logger.LogInformation("Home Controller / Index ");
            try
            {
                return this.View();
            }
            catch 
            {
                throw;
            }
        }

        /// <summary>
        /// Subscriptionses this instance.
        /// </summary>
        /// <returns> The <see cref="IActionResult" />.</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Subscriptions()
        {
            this.logger.LogInformation("Home Controller / Subscriptions");
            SummarySubscriptionViewModel summarySubscription = new();

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
            catch 
            {
                throw;

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
        [HttpGet]
        public IActionResult SubscriptionDetails(string id)
        {
            var subscriptionDetail = new SubscriptionModel();

            this.logger.LogInformation("Home Controller / SubscriptionLogDetail : subscriptionId: {Id}", JsonSerializer.Serialize(id));

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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                throw;

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
        [HttpGet]
        public IActionResult Error()
        {
            var exceptionDetail = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return this.View(exceptionDetail?.Error);
        }
        [Authorize]
        [HttpGet]
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                throw;

            }


            return this.View(subscriptionDetail);
        }
        [Authorize]
        // POST: Subscription/Create
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult NewSubscriptionAction(SubscriptionModel subscription)
        {
            if(subscription ==null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
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
                        subscription.id = subscription.id.Replace("/", "|",StringComparison.OrdinalIgnoreCase);

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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during Saving new Subscription with Id {HttpUtility.HtmlEncode(subscription.id)} to Db: {ex.Message}");
                throw;

            }


            return this.View(subscriptionDetail);
        }
        [Authorize]
        [HttpGet]
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                throw;
            }
            return this.RedirectToAction("Subscriptions");
        }
        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
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
                        this.applicationLogService.AddApplicationLog($"Completed Saving  Subscription Id: {HttpUtility.HtmlEncode(subscription?.id)}");
                    }

                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }


            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during Saving  Subscription with Id {HttpUtility.HtmlEncode(subscription?.id)} to Db: {ex.Message}");
                throw;
            }
            return this.RedirectToAction("Subscriptions");
        }
        [Authorize]
        [HttpGet]
        public ActionResult DeleteSubscription(string subscriptionId)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }    
            this.logger.LogInformation("Home Controller / Delete Subscription Action");
            this.applicationLogService.AddApplicationLog($"Start Deleting Subscription from Db: {HttpUtility.HtmlEncode(subscriptionId)}");
            try
            {

                if (this.User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    if (ModelState.IsValid)
                    {
                        var schedulerTasks = this.schedulerService.GetSchedulersTasksBySubscription(subscriptionId.Replace("|", "/", StringComparison.OrdinalIgnoreCase));
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during deleting  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");
                throw;

            }
            return this.RedirectToAction(nameof(this.Subscriptions));

        }
        [Authorize]
        [HttpGet]
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during unsubscribe  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");
                throw;
            }
            return this.RedirectToAction(nameof(this.Subscriptions));
        }
        [Authorize]
        [HttpGet]
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
                this.logger.LogError("Message:{Message} :: {InnerException}   ", ex.Message, ex.InnerException);
                this.applicationLogService.AddApplicationLog($"Error during subscribe  Subscription with Id {HttpUtility.HtmlEncode(subscriptionId)} to Db: {ex.Message}");
                throw;
            }
            return this.RedirectToAction(nameof(this.Subscriptions));
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            return this.View();
        }

    }
}