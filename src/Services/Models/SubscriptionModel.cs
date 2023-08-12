using System;

namespace ManagedApplicationScheduler.Services.Models
{
    public class SubscriptionModel
    {

        /// <summary>
        /// Gets or sets the identifier, which is the application resource identifier.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the publisher identifier.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the Product identifier.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Get Set Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Get or Set Provision State
        /// </summary>
        public string ProvisionState { get; set; }

        /// <summary>
        /// Get or Set Provision Time
        /// </summary>
        public DateTime ProvisionTime { get; set; }

        /// <summary>
        /// Gets or sets the resource usage identifier.
        /// </summary>
        /// 
        public string ResourceUsageId { get; set; }
        /// <summary>
        /// process Status
        /// </summary>
        public string SubscriptionStatus { get; set; }

        public string Dimension { get; set; }

        public string ErrorMessage { get; set; }
    }
}
