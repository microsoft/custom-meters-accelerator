using ManagedApplicationScheduler.Services.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace ManagedApplicationScheduler.Services.Utilities
{
    public class AzureAppOfferApi
    {
        private string apiAllProduct = "https://api.partner.microsoft.com/v1.0/ingestion/products?$filter=resourceType eq 'AzureApplication' and ExternalIDs/Any(i:i/Type eq 'AzureOfferId' and i/Value eq '%OFFERID%')";
        private string apiProductVariants = "https://api.partner.microsoft.com/v1.0/ingestion/products/%PRODUCTID%/variants/";
        private string apiProductBranches = "https://api.partner.microsoft.com/v1.0/ingestion/products/%PRODUCTID%/branches/getByModule(module=availability)";
        private string apiProductFeatures = "https://api.partner.microsoft.com/v1.0/ingestion/products/%PRODUCTID%/featureAvailabilities/getByInstanceID(instanceID=%INSTANCEID%)";
        private string token;
        private HttpClient httpClient = new HttpClient();
        public AzureAppOfferApi(string token)
        {
            this.token = token;
            this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<string> getProductDims(string offerId, string planName)
        {
            string dims = "";
            string productId = await getProductIdAsync(offerId.Replace("-preview", ""));
            if (productId != null)
            {
                string variantsId = await getVariantsId(productId, planName);
                if (variantsId != null)
                {
                    string instanceId = await getInstanceId(productId, variantsId);
                    if (instanceId != null)
                    {
                        string dimlist = await getProductFeature(productId, instanceId);
                        if (dimlist != null)
                        {
                            dims = dimlist;
                        }
                    }
                }
            }



            return dims;
        }

        private async Task<string> getProductIdAsync(string offerId)
        {
            var url = this.apiAllProduct.Replace("%OFFERID%", offerId);
            var response = await httpClient.GetAsync(url).ConfigureAwait(continueOnCapturedContext: false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);

            var products = JsonSerializer.Deserialize<ProductModel>(responseBody);
            if (products != null)
            {
                return products.value[0].id;
            }
            return "";
        }

        private async Task<string> getVariantsId(string productId, string planName)
        {


            var url = this.apiProductVariants.Replace("%PRODUCTID%", productId);
            var response = await httpClient.GetAsync(url).ConfigureAwait(continueOnCapturedContext: false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);

            var items = JsonSerializer.Deserialize<ProductVariantModel>(responseBody);

            foreach (var item in items.value)
            {
                if (item.friendlyName == planName)
                {
                    return item.id;
                }
            }

            return "";
        }

        private async Task<string> getInstanceId(string productId, string variantsId)
        {


            var url = this.apiProductBranches.Replace("%PRODUCTID%", productId);
            var response = await httpClient.GetAsync(url).ConfigureAwait(continueOnCapturedContext: false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);

            var items = JsonSerializer.Deserialize<ProductBranchModel>(responseBody);
            foreach (var item in items.value)
            {
                if (item.variantID == variantsId)
                {
                    return item.currentDraftInstanceID;
                }
            }
            return "";
        }

        private async Task<string> getProductFeature(string productId, string instanceId)
        {
            var dimsList = new List<string>();
            var url = this.apiProductFeatures.Replace("%PRODUCTID%", productId).Replace("%INSTANCEID%", instanceId);
            var response = await httpClient.GetAsync(url).ConfigureAwait(continueOnCapturedContext: false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);

            var items = JsonSerializer.Deserialize<ProductFeatureModel>(responseBody);
            foreach (var item in items.value)
            {
                if (item.id == instanceId)
                {
                    foreach (var dim in item.customMeters)
                    {
                        if (dim.isEnabled)
                        {
                            dimsList.Add(dim.id);
                        }
                    }
                }
            }

            if (dimsList.Count > 0)
            {
                return string.Join<string>("|", dimsList);
            }

            return "";
        }

    }
}
