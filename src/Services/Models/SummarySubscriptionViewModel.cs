using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    public class SummarySubscriptionViewModel
    {

        public List<SubscriptionViewModel> Subscriptions { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }

    }
}
