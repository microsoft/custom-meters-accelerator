using System;

namespace ManagedApplicationScheduler.DataAccess.Entities
{
    public partial class ApplicationLog
    {
        public string? id { get; set; }
        public DateTime? ActionTime { get; set; }
        public string? LogDetail { get; set; }
        public string? PartitionKey { get; set; }
    }
}