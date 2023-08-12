using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{

    public class ProductVariantModel
    {
        public List<VariantModel> value { get; set; }
    }

    public class VariantModel
    {
        public string resourceType { get; set; }
        public string state { get; set; }
        public string friendlyName { get; set; }
        public string conversionPaths { get; set; }
        public string externalID { get; set; }
        public string subType { get; set; }
        public string id { get; set; }
    }
}
