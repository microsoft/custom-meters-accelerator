﻿using System.Net;
using System.Net.Mail;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Contracts;
using ManagedApplicationScheduler.Services.Models;

namespace ManagedApplicationScheduler.Services.Services;

/// <summary>
/// Service to send emails using SMTP settings.
/// </summary>
/// <seealso cref="IEmailService" />
public class SMTPEmailService : IEmailService
{
    /// <summary>
    /// The application configuration repository.
    /// </summary>
    private readonly ApplicationConfigurationService applicationConfigurationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SMTPEmailService"/> class.
    /// </summary>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    public SMTPEmailService(IApplicationConfigurationRepository applicationConfigRepository)
    {
        this.applicationConfigurationService = new ApplicationConfigurationService(applicationConfigRepository);
    }

    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="emailContent">Content of the email.</param>
    public void SendEmail(EmailContentModel emailContent)
    {
        MailMessage mail = new MailMessage();
        if (!string.IsNullOrEmpty(emailContent.ToEmails) || !string.IsNullOrEmpty(emailContent.BCCEmails))
        {
            mail.From = new MailAddress(emailContent.FromEmail);
            mail.IsBodyHtml = true;
            mail.Subject = emailContent.Subject;
            mail.Body = emailContent.Body;

            string[] toEmails = emailContent.ToEmails.Split(';');
            foreach (string multimailid in toEmails)
            {
                mail.To.Add(new MailAddress(multimailid));
            }

            if (!string.IsNullOrEmpty(emailContent.BCCEmails))
            {
                foreach (string multimailid1 in toEmails)
                {
                    mail.Bcc.Add(new MailAddress(multimailid1));
                }
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = emailContent.SMTPHost;
            smtp.Port = emailContent.Port;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                emailContent.UserName, emailContent.Password);
            smtp.EnableSsl = emailContent.SSL;
            smtp.Send(mail);
        }
    }
}