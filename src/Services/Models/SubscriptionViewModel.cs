using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    public class SubscriptionViewModel
    {

        public string id { get; set; }
        public string Product { get; set; }


        public string AppId { get; set; }

        public string PlanId { get; set; }

        public string Subscription { get; set; }
        public string ProvisionState { get; set; }

        public List<MeteredUsageResultModel> meteringUsageResultModels { get; set; }


        public string ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }

        public string SubscriptionStatus { get; set; }
    }
}
