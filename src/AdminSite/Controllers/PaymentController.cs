﻿using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Collections.Specialized.BitVector32;
using System.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace ManagedApplicationScheduler.AdminSite.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class PaymentController : BaseController
    {
        private readonly PaymentService paymentService;
        private readonly PlanService planService;
        private readonly ApplicationLogService applicationLogService;
        private readonly ILogger<PaymentController> logger;
        private readonly ManagedAppClientConfiguration config;
        public PaymentController(ManagedAppClientConfiguration config, ILogger<PaymentController> logger, IPaymentRepository paymentRepository, IPlanRepository planRepository, IApplicationLogRepository applicationLogRepository, IScheduledTasksRepository scheduledTasksRepository)
        {
            this.applicationLogService = new ApplicationLogService(applicationLogRepository);
            this.paymentService = new PaymentService(paymentRepository, scheduledTasksRepository);
            this.planService = new PlanService(planRepository);
            this.logger = logger;
            this.config = config;
        }
        [HttpGet]
        public IActionResult Index()
        {
            this.logger.LogInformation("Get All Payment Controller");

            if (this.User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";
                var model = this.paymentService.GetAllPayment();
                return View(model);
            }
            return this.RedirectToAction(nameof(this.Index));

        }

        [HttpPost]
        public async Task<IActionResult> FetchAllPlans()
        {
            try
            {
                var creds = new Azure.Identity.ClientSecretCredential(config.PC_TenantId, config.PC_ClientID, config.PC_ClientSecret);
                var result = await creds.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { config.PC_Scope }), System.Threading.CancellationToken.None).ConfigureAwait(false);
                var token = result.Token;
                await this.planService.GetAllMeteredPlansAsync(token);
            }
            catch (Exception ex)
            {
                this.applicationLogService.AddApplicationLog($"Error: {ex.Message}");
                this.applicationLogService.AddApplicationLog($"Error: {ex.InnerException}");
                return BadRequest();
            }
            return Ok();

        }


        [HttpGet]
        public IActionResult GetProductPlans(string id)
        {

            List<PlanModel> plans;

            var value = HttpContext.Session.GetString("plans");

            if (string.IsNullOrEmpty(value))
            {
                plans = this.planService.GetPlanListByOfferId(id);
            }
            else
            {
                plans = JsonConvert.DeserializeObject<List<PlanModel>>(value);
                plans = plans.Where(s => s.Product == id).ToList();
            }


            if (plans.Count > 0)
            {
                // Create Dimension Dropdown list
                List<SelectListItem> selectedList = new();
                foreach (var item in plans)
                {
                    selectedList.Add(new SelectListItem()
                    {
                        Text = item.PlanName,
                        Value = item.Name,
                    });
                }

                return Json(selectedList);

            }

            return this.PartialView("Error", "Can not find any metered plan related to selected product");
        }

        [HttpGet]
        public IActionResult GetPlanDimensions(string offerId, string planId)
        {

            List<PlanModel> plans;
            PlanModel plan;

            var value = HttpContext.Session.GetString("plans");

            if (string.IsNullOrEmpty(value))
            {
                plan = this.planService.GetPlanByOfferIdPlanId(planId, offerId);
            }
            else
            {
                plans = JsonConvert.DeserializeObject<List<PlanModel>>(value);
                plan = plans.Where(s => s.Product == offerId && s.Name == planId).FirstOrDefault();
            }




            if (plan != null)
            {
                // Create Dimension Dropdown list
                List<SelectListItem> selectedList = new();
                var dims = plan.Dimension.Split('|');
                foreach (var dim in dims)
                {
                    selectedList.Add(new SelectListItem()
                    {
                        Text = dim,
                        Value = dim,
                    });
                }

                return Json(selectedList);

            }

            return this.PartialView("Error", "Can not find any metered dimension related to selected plan");
        }

        private string DoesPaymentExists(PaymentFormModel paymentFormModel)
        {
            string id = "";
            bool paymentExist = false;
            var existingPayments = paymentService.GetPaymentByOfferByPlanByDimByType(paymentFormModel.SelectedProduct, paymentFormModel.SelectedPlan, paymentFormModel.SelectedDimension, paymentFormModel.SelectedPaymentType);

            foreach (var existingPayment in existingPayments)
            {
                if (existingPayment.PaymentName == paymentFormModel.PaymentName)
                {
                    
                    id = existingPayment.id;
                    break;
                    
                }

                if (existingPayment.PaymentType == "Upfront")
                {
                    id = existingPayment.id;
                    break;
                }
                if ((existingPayment.PaymentType == "Milestone") && (existingPayment.StartDate == paymentFormModel.StartDate.AddHours(paymentFormModel.TimezoneOffset)))
                {
                    id = existingPayment.id;
                    break;
                }
            }

            if (!paymentExist)
            {

                existingPayments = paymentService.GetPaymentByName(paymentFormModel.PaymentName);

                foreach (var existingPayment in existingPayments)
                {
                    if (existingPayment.PaymentName == paymentFormModel.PaymentName)
                        id = existingPayment.id;
                    break;
                    ;
                }
            }

            return id;
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult NewPayment(PaymentFormModel paymentFormModel)
        {
            if (paymentFormModel == null)
            {
                throw new ArgumentNullException(nameof(paymentFormModel));
            }
            string id = DoesPaymentExists(paymentFormModel);
            if (!string.IsNullOrEmpty(id))
            {

                var existingPayment = PreparePaymentFormModel(id);

                paymentFormModel.PlanList = existingPayment.PlanList;
                paymentFormModel.SelectedProduct = existingPayment.SelectedProduct;
                paymentFormModel.ProductList = existingPayment.ProductList;
                paymentFormModel.DimensionsList = existingPayment.DimensionsList;
                paymentFormModel.PaymentTypeList = existingPayment.PaymentTypeList;
                paymentFormModel.Quantity = existingPayment.Quantity;
                paymentFormModel.Error = "Payment already exist! Avoid duplicate Payment in order to avoid duplicate meters submission!";
                return this.View(paymentFormModel);

            }
            try
            {
                this.applicationLogService.AddApplicationLog($"Start Adding new Task : {HttpUtility.HtmlEncode(paymentFormModel)}");
                // check if payment exist
                PaymentModel payment = new()
                {
                    id = Guid.NewGuid().ToString(),
                    PaymentType = paymentFormModel.SelectedPaymentType,
                    PaymentName = paymentFormModel.PaymentName,
                    PlanId = paymentFormModel.SelectedPlan,
                    OfferId = paymentFormModel.SelectedProduct,
                    Dimension = paymentFormModel.SelectedDimension,
                    Quantity = Convert.ToDouble(paymentFormModel.Quantity),
                    StartDate = paymentFormModel.StartDate.AddHours(paymentFormModel.TimezoneOffset),
                    
                };
                this.paymentService.SavePayment(payment);
                this.applicationLogService.AddApplicationLog($"Completed Adding new Task : {HttpUtility.HtmlEncode(paymentFormModel.PaymentName)}");

                return this.RedirectToAction(nameof(this.NewPayment));


            }
            catch (Exception ex)
            {
                this.logger.LogError("{Message}", ex.Message);
                this.applicationLogService.AddApplicationLog($"Error during Saving Task with Name {HttpUtility.HtmlEncode(paymentFormModel.PaymentName)} to Db: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public IActionResult NewPayment(string id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                if (id != null)
                {
                    this.logger.LogInformation("Payment Controller / New Payment Item Details:  Id {Id}", HttpUtility.HtmlEncode(id));
                    this.applicationLogService.AddApplicationLog($"Start New Payment with Id : {HttpUtility.HtmlEncode(id)}");
                }
                else
                {
                    this.logger.LogInformation("Payment Controller / New Payment Item");
                    this.applicationLogService.AddApplicationLog($"Start New Payment");
                }
                this.TempData["ShowWelcomeScreen"] = "True";
                try
                {
                    PaymentFormModel model = PreparePaymentFormModel(id);
                    return this.View(model);
                }
                catch (Exception ex)
                {
                    this.logger.LogError("{Message}", ex.Message);
                    throw;
                }
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }


        }
        [HttpGet]
        public IActionResult DeletePayment(string id)
        {
            this.logger.LogInformation("Payment Controller / Remove Payment Item Details:  Id {Id}", HttpUtility.HtmlEncode(id));
            this.applicationLogService.AddApplicationLog($"Start Deleting Payment with Id : {HttpUtility.HtmlEncode(id)}");
            try
            {
                this.paymentService.DeletePayment(id);
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

        private PaymentFormModel PreparePaymentFormModel(string id)
        {
            var payment =  this.paymentService.GetPaymentID(id);
            PaymentFormModel model = new();
            var productlist = this.planService.GetOfferList();

            var allplans = this.planService.GetAllPlan();
            this.HttpContext.Session.SetString("plans", JsonConvert.SerializeObject(allplans));
            // Create Dropdown list
            List<SelectListItem> offerlist = new();
            List<SelectListItem> planlist = new();
            List<SelectListItem> DimensionsList = new();

            List<SelectListItem> paymentTypelist = new()
                    {
                        new SelectListItem()
                        {
                            Text = "Upfront",
                            Value = "Upfront",
                        },
                        new SelectListItem()
                        {
                            Text = "Milestone",
                            Value = "Milestone",
                        }
                    };
            // Create Subscription Dropdown list
            foreach (var item in productlist)
            {
                offerlist.Add(new SelectListItem()
                {
                    Text = item,
                    Value = item,
                });
            }

            if (payment != null)
            {
                model.SelectedProduct = payment.OfferId;
                model.SelectedPlan = payment.PlanId;
                model.SelectedDimension = payment.Dimension;
                model.SelectedPaymentType = payment.PaymentType;
                var plans = this.planService.GetPlanListByOfferId(payment.OfferId);
                var dims = this.planService.GetPlanByOfferIdPlanId(payment.PlanId, payment.OfferId).Dimension.Split("|");

                foreach (var item in plans)
                {
                    planlist.Add(new SelectListItem() { Value = item.Name, Text = item.PlanName });
                }
                foreach (var item in dims)
                {
                    DimensionsList.Add(new SelectListItem() { Value = item, Text = item });
                }
                model.Quantity = payment.Quantity;

                if (payment.PaymentType == "Upfront")
                {
                    model.IsUpfrontPayment = true;
                }
            }

            else
            {
                model.SelectedProduct = "-- Select Offer --";
                model.SelectedPlan = "-- Select Plan --";
                model.SelectedDimension = "-- Select Dimension --";
                model.SelectedPaymentType = "-- Select PaymentType --";
            }
            // Create Plan Dropdown list
            model.DimensionsList = new SelectList(DimensionsList, "Value", "Text");
            model.PlanList = new SelectList(planlist, "Value", "Text");
            model.PaymentTypeList = new SelectList(paymentTypelist, "Value", "Text");
            model.ProductList = new SelectList(offerlist, "Value", "Text");

            return model;
        }
    }

}
