// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace ManagedApplicationScheduler.DataAccess.Entities
{

    public class UsageResult
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the usage posted date.
        /// </summary>
        /// <value>
        /// The usage posted date.
        /// </value>
        public DateTime? UsagePostedDate { get; set; }

        /// <summary>
        /// Gets or sets the usage event identifier.
        /// </summary>
        /// <value>
        /// The usage event identifier.
        /// </value>
        public string UsageEventId { get; set; }

        /// <summary>
        /// Gets or sets the message time.
        /// </summary>
        /// <value>
        /// The message time.
        /// </value>
        public DateTime MessageTime { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        /// The resource identifier.
        /// </value>
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the ScheduledTaskId.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string ScheduledTaskName { get; set; }


        public string PartitionKey { get; set; }

        public string ResourceUri { get; set; }

        public string Message { get; set; }
    }
}