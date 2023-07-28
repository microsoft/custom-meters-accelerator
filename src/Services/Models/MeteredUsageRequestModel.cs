﻿using System;

namespace ManagedApplicationScheduler.Services.Models
{
    /// <summary>
    /// The usage event definition.
    /// </summary>
    public class MeteredUsageRequestModel
    {
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
        public DateTime EffectiveStartTime { get; set; }

        /// <summary>
        /// Plan associated with the purchased offer.
        /// </summary>
        public string PlanId { get; set; }
    }
}
