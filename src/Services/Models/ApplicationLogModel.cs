using System;

namespace ManagedApplicationScheduler.Services.Models
{
    public partial class ApplicationLogModel
    {
        public string id { get; set; }
        public DateTime? ActionTime { get; set; }
        public string LogDetail { get; set; }
        public string PartitionKey { get; set; }
    }
}