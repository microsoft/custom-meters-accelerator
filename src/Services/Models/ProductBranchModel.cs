using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    public class ProductBranchModel
    {
        public List<BranchModel> value { get; set; }
    }

    public class BranchModel
    {
        public string resourceType { get; set; }
        public string friendlyName { get; set; }
        public string type { get; set; }
        public string module { get; set; }
        public string currentDraftInstanceID { get; set; }
        public string variantID { get; set; }
    }

}
