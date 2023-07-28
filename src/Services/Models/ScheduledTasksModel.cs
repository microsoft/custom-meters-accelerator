using System;
using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    /// <summary>
    /// The usage event definition.
    /// </summary>
    public class ScheduledTasksModel
    {
        /// <summary>
        /// Get or Set Id
        /// </summary>
        public string id { get; set; }


        /// <summary>
        /// Get or Set Scheduled Task Name.
        /// </summary>
        public string ScheduledTaskName { get; set; }


        /// <summary>
        /// Identifier of the resource against which usage is emitted.
        /// </summary>
        public string ResourceUri { get; set; }

        /// <summary>
        /// The quantity of the usage.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Dimension identifier.
        /// </summary>
        public string Dimension { get; set; }

        /// <summary>
        /// Time in UTC when the usage event occurred.
        /// </summary>
        public DateTime? NextRunTime { get; set; }
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Plan associated with the purchased offer.
        /// </summary>
        public string PlanId { get; set; }

        public string Frequency { get; set; }


        public string PartitionKey { get; set; }


        public string Status { get; set; } //Enabled, Canceled, Completed, Error


        public List<MeteredUsageResultModel> MeteredUsageResult { get; set; }

    }
}
