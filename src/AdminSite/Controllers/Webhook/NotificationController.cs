using Azure.Identity;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ManagedApplicationScheduler.AdminSite.Controllers.Webhook
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/resource")]
    public class NotificationController : ControllerBase
    {

        private readonly ManagedAppClientConfiguration config;
        private readonly SubscriptionService subscriptionService;
        private readonly ApplicationLogService applicationLogService;
        public NotificationController(ManagedAppClientConfiguration config, ISubscriptionsRepository subscriptionsRepository, IApplicationLogRepository applicationLogRepository)
        {
            subscriptionService = new SubscriptionService(subscriptionsRepository);
            this.config = config;
            this.applicationLogService = new ApplicationLogService(applicationLogRepository);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostAsync(NotificationDefinitionModel notificationDefinition, string sig)
        {
            if(notificationDefinition == null)
            {
                throw new ArgumentNullException(nameof(notificationDefinition));
            }
            if (config.Signature == sig)
            {
                var creds = new ClientSecretCredential(config.PC_TenantId, config.PC_ClientID, config.PC_ClientSecret);
                var result = await creds.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { config.PC_Scope }), System.Threading.CancellationToken.None).ConfigureAwait(false);
                var token = result.Token;
                

                // If provisioning of a marketplace application instance is successful, we persist a billing entry to be picked up by the chron metric emitting job
                if (notificationDefinition.EventType == "PUT" && notificationDefinition.ProvisioningState == "Succeeded" && notificationDefinition.BillingDetails?.ResourceUsageId != null)
                    {
                        this.applicationLogService.AddApplicationLog($"Successful Subscription added {notificationDefinition.ApplicationId} ");
                        var subscription = new SubscriptionModel
                        {
                            // CosmosDB does not support forward slashes in the id.
                            id = notificationDefinition.ApplicationId.Replace("/", "|",StringComparison.OrdinalIgnoreCase),
                            PlanId = notificationDefinition.Plan.Name,
                            Product = notificationDefinition.Plan.Product,
                            Publisher = notificationDefinition.Plan.Publisher,
                            Version = notificationDefinition.Plan.Version,
                            ProvisionState = notificationDefinition.ProvisioningState,
                            ProvisionTime = DateTime.UtcNow,
                            ResourceUsageId = notificationDefinition.BillingDetails.ResourceUsageId,
                            SubscriptionStatus = "Subscribed"
                        };

                        try
                        {
                            this.applicationLogService.AddApplicationLog($"Get dims list for Product {subscription.Product} with plan {subscription.PlanId}");
                            var azureOfferApi = new AzureAppOfferApi(token);
                            subscription.Dimension = await azureOfferApi.getProductDims(subscription.Product, subscription.PlanId).ConfigureAwait(false);

                            this.applicationLogService.AddApplicationLog($"Found dims : {subscription.Dimension}");
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"Error during getting Product Dims.  {ex.Message}");
                            this.applicationLogService.AddApplicationLog($"Error during getting Product Dims.  {ex.Message}");
                        throw;
                        }



                        subscriptionService.SaveSubscription(subscription);

                        Console.WriteLine($"Successfully inserted the entry in CosmosDB for the application {notificationDefinition.ApplicationId}");
                        this.applicationLogService.AddApplicationLog($"Successfully inserted the entry in CosmosDB for the application {notificationDefinition.ApplicationId}");
                    }
                    else if (notificationDefinition.EventType == "DELETE" && notificationDefinition.ProvisioningState == "Deleted")
                    {
                        // On successful deletion of a marketplace application instance try to delete a billing entry in case one was created
                        this.applicationLogService.AddApplicationLog($"Received Delete for Subscription {notificationDefinition.ApplicationId} ");
                        var subscription = new SubscriptionModel
                        {
                            // CosmosDB does not support forward slashes in the id.
                            id = notificationDefinition.ApplicationId.Replace("/", "|",StringComparison.OrdinalIgnoreCase),
                            PlanId = notificationDefinition.Plan.Name,
                            Product = notificationDefinition.Plan.Product,
                            Publisher = notificationDefinition.Plan.Publisher,
                            Version = notificationDefinition.Plan.Version,
                            ProvisionState = notificationDefinition.ProvisioningState,
                            ProvisionTime = DateTime.UtcNow,
                            ResourceUsageId = notificationDefinition.BillingDetails.ResourceUsageId
                        };
                            subscriptionService.DeleteSubscription(subscription);
                            Console.WriteLine($"Successfully deleted the entry in CosmosDB for the application {notificationDefinition.ApplicationId}");
                            this.applicationLogService.AddApplicationLog($"Successfully deleted the entry in CosmosDB for the application {notificationDefinition.ApplicationId}");

                    }
                
                return Ok();
            }
            else { return Forbid(); }


        }



    }
}

