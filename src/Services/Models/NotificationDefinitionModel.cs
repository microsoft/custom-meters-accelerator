using Newtonsoft.Json;

namespace ManagedApplicationScheduler.Services.Models
{
    /// <summary>
    /// The notification definition.
    /// </summary>
    public class NotificationDefinitionModel
    {
        /// <summary>
        /// Gets or sets the application resource identifier.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the provisioning state.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ProvisioningState { get; set; }

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public PlanModel Plan { get; set; }

        /// <summary>
        /// Gets or sets the billing details.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public BillingDetailsDefinitionModel BillingDetails { get; set; }
    }
}
