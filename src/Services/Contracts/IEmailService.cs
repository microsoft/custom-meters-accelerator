using ManagedApplicationScheduler.Services.Models;

namespace ManagedApplicationScheduler.Services.Contracts;

/// <summary>
/// Contract for Emails service smtp /send grid.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="emailContent">Content of the email.</param>
    void SendEmail(EmailContentModel emailContent);
}