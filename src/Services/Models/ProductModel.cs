using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    public class ExternalID
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ProductModel
    {
        public List<ProductValue> value { get; set; }
    }

    public class ProductValue
    {
        public string resourceType { get; set; }
        public string name { get; set; }
        public List<ExternalID> externalIDs { get; set; }
        public bool isModularPublishing { get; set; }
        public string id { get; set; }
    }

}
