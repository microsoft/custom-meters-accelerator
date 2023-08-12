namespace ManagedApplicationScheduler.DataAccess.Entities
{

    public partial class ApplicationConfiguration
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string PartitionKey { get; set; }
    }
}