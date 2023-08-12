namespace ManagedApplicationScheduler.DataAccess.Entities
{
    public class Subscription
    {

        /// <summary>
        /// Gets or sets the identifier, which is the application resource identifier.
        /// </summary>
        public string id { get; set; }

        public string PlanId { get; set; }

        public string Publisher { get; set; }

        public string Product { get; set; }

        public string Version { get; set; }

        public string ProvisionState { get; set; }

        public DateTime ProvisionTime { get; set; }

        public string ResourceUsageId { get; set; }

        public string SubscriptionStatus { get; set; }
        public string PartitionKey { get; set; }
        public string Dimension { get; set; }
    }
}
