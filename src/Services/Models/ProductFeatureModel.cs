using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManagedApplicationScheduler.Services.Models
{
    public class ProductFeatureModel
    {
        public List<FeatureModel> value { get; set; }
    }


    public class FeatureModel
    {
        public string resourceType { get; set; }
        public string visibility { get; set; }
        public List<Property> properties { get; set; }
        public List<PriceSchedule> priceSchedules { get; set; }
        public List<CustomMeter> customMeters { get; set; }

        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string id { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CustomMeter
    {
        public bool isEnabled { get; set; }
        public string id { get; set; }
        public string uniqueID { get; set; }
        public string displayName { get; set; }
        public string unitOfMeasure { get; set; }
        public double priceInUsd { get; set; }
        public List<IncludedBaseQuantity> includedBaseQuantities { get; set; }
    }

    public class IncludedBaseQuantity
    {
        public string recurringUnit { get; set; }
        public bool isInfinite { get; set; }
        public double quantity { get; set; }
    }

    public class PriceCadence
    {
        public string type { get; set; }
        public int value { get; set; }
    }

    public class PriceSchedule
    {
        public bool isBaseSchedule { get; set; }
        public string friendlyName { get; set; }
        public List<Schedule> schedules { get; set; }
        public List<string> marketCodes { get; set; }
    }

    public class Property
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class RetailPrice
    {
        public double openPrice { get; set; }
        public string currencyCode { get; set; }
    }

    public class Schedule
    {
        public RetailPrice retailPrice { get; set; }
        public string pricingModel { get; set; }
        public PriceCadence priceCadence { get; set; }
    }



}
