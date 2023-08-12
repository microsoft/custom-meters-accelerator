using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using System;

namespace ManagedApplicationScheduler.Services.Helpers
{

    /// <summary>
    /// Email Helper.
    /// </summary>
    public class EmailHelper
    {
        private readonly ApplicationConfigurationService applicationConfigurationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHelper"/> class.
        /// </summary>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        public EmailHelper(IApplicationConfigurationRepository applicationConfigRepository)
        {
            this.applicationConfigurationService = new ApplicationConfigurationService(applicationConfigRepository);
        }
        /// <summary>
        /// Prepares the content of the scheduler email.
        /// </summary>
        /// <param name="subscriptionName">The subscription Name.</param>
        /// <param name="schedulerTaskName">scheduler Task Name.</param>
        /// <param name="responseJson">response Json.</param>
        /// <param name="status">The subscription status.</param>
        /// <returns>
        /// Email Content Model.
        /// </returns>
        /// <exception cref="Exception">Error while sending an email, please check the configuration.
        /// or
        /// Error while sending an email, please check the configuration.</exception>
        public EmailContentModel PrepareMeteredEmailContent(string schedulerTaskName, String subscriptionName, string status, string responseJson)
        {
            var emailBody = this.applicationConfigurationService.GetValueByName(status+"_Email");
            var emailSubject = this.applicationConfigurationService.GetValueByName(status + "_Subject");
            string toReceipents = this.applicationConfigurationService.GetValueByName("SchedulerEmailTo");
            if (string.IsNullOrEmpty(toReceipents))
            {
                throw new Exception(" Error while sending an email, please check the configuration. ");
            }
            var body = emailBody.Replace("****SubscriptionName****", subscriptionName).Replace("****SchedulerTaskName****", schedulerTaskName).Replace("****ResponseJson****", responseJson); ;
            return FinalizeContentEmail(emailSubject, body, string.Empty, string.Empty, toReceipents, false);
        }
        private EmailContentModel FinalizeContentEmail(string subject, string body, string ccEmails, string bcEmails, string toEmails, bool copyToCustomer)
        {
            EmailContentModel emailContent = new EmailContentModel();
            emailContent.BCCEmails = bcEmails;
            emailContent.CCEmails = ccEmails;
            emailContent.ToEmails = toEmails;
            emailContent.IsActive = false;
            emailContent.CopyToCustomer = copyToCustomer;
            emailContent.Subject = subject;
            emailContent.Body = body;
            emailContent.FromEmail = this.applicationConfigurationService.GetValueByName("SMTPFromEmail");
            emailContent.Password = this.applicationConfigurationService.GetValueByName("SMTPPassword");
            emailContent.SSL = bool.Parse(this.applicationConfigurationService.GetValueByName("SMTPSslEnabled"));
            emailContent.UserName = this.applicationConfigurationService.GetValueByName("SMTPUserName");
            emailContent.Port = int.Parse(this.applicationConfigurationService.GetValueByName("SMTPPort"));
            emailContent.SMTPHost = this.applicationConfigurationService.GetValueByName("SMTPHost");
            return emailContent;
        }


    }
}