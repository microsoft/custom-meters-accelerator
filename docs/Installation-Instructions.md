# Install the Azure Marketplace SaaS Accelerator using Azure Cloud Shell

<!-- no toc -->
- [Basic installation script](#basic-installation-script)
  - [Optional install script parameters](#optional-install-script-parameters)
- [Install script parameter descriptions](#install-script-parameter-descriptions)


## Basic installation script

You can install the Azure Application Billing Scheduler code using a __single command__ line within the Azure Portal Cloud Shell.

> Note: use the [Azure Cloud Shell](https://shell.azure.com)'s PowerShell shell, not the default bash shell. You can select the shell via the drop-down in the top left corner.

Copy the following section to an editor and update it to match your company preference.

- Replace `SOME-UNIQUE-STRING` with your Team name or some other meaningful name for your depth. (Ensure that the final name does not exceed 21 characters)
- Replace `user@email.com` with a valid email from your org that will use the portal for the first time. Once deployed, this account will be able to login to the administration panel and give access to more users.
- Replace `SOME-RG-NAME` with a value that matches your company's naming conventions for resource groups
- [Optional] Replace `East US` with a region closest to you.

``` powershell
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef; `
git clone https://github.com/microsoft/azure-app-billing-scheduler  --depth 1; `
cd ./azure-app-billing-scheduler/Deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "SOME-RG-NAME" `
 -PublisherAdminUsers "user@email.com" `
 -Location "East US" 
 ```

The script above will perform the following actions.

- Create required App Registration for Marketplace Metered API authentication
- Deploy required infrastructure in Azure for hosting admin portal, Notification API endpoint, KeyVault and CosmosDB


### Optional install script parameters

 The following are optional parameters that you can include in the deployment  (see parameter description below for details).
 
 ``` powershell
 -TenantID "xxxx-xxx-xxx-xxx-xxxx" `
 -AzureSubscriptionID "xxx-xx-xx-xx-xxxx" `
 -ADApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -ADApplicationSecret "xxxx-xxx-xxx-xxx-xxxx" `
 -PCADApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -PCADApplicationSecret "xxxx-xxx-xxx-xxx-xxxx" `
 -PCTenantID "xxxx-xxx-xxx-xxx-xxxx" `
 -LogoURLpng "https://company_com/company_logo.png" `
 -LogoURLico "https://company_com/company_logo.ico" `
  -Quiet
 ```

## Install script parameter descriptions

| Parameter | Description |
|-----------| -------------|
| WebAppNamePrefix | _[required]_ A unique prefix used for creating web applications. Example: `contoso` |
| ResourceGroupForDeployment | Name of the resource group to deploy the resources. Default: `WebAppNamePrefix` value |
| Location | _[required]_ Location of the resource group |
| PublisherAdminUsers | _[required]_ Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal |
| TenantID | The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center. If value not provided, you will be asked to select the tenant during deployment |
| AzureSubscriptionID | Id of subscription where the resources will be deployed. Subscription must be part of the Tenant Provided. If value not provided, you will be asked to select the subscription during deployment. |
| ADApplicationID | The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center. If value not provided, a new application will be created. |
| ADApplicationID Secret | The secret for the AAD Application ID. |
|PC TenantID | The value should match the value provided for Partner Center Tenants |
| PC ADApplicationID | The value should match the value provided for Partner Center -> User Management -> Managed Application section|
| PC ADApplicationID Secret | The secret for the PC Application ID |
| LogoURLpng | The url of the company logo image in .png format with a size of 96x96 to be used on the website |
| LogoURLico | The url of the company logo image in .ico format |
| Quiet | Disable verbose output when running the script

