using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;
using ManagedApplicationScheduler.Services.Models;

namespace ManagedApplicationScheduler.Services.Services;

/// <summary>
/// Application Log Service.
/// </summary>
public class ApplicationConfigurationService
{
    /// <summary>
    /// The application log repository.
    /// </summary>
    private readonly IApplicationConfigurationRepository applicationConfigurationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLogService"/> class.
    /// </summary>
    /// <param name="applicationLogRepository">The application log repository.</param>
    public ApplicationConfigurationService(IApplicationConfigurationRepository applicationConfigurationRepository)
    {
        this.applicationConfigurationRepository = applicationConfigurationRepository;
    }

    
    public void UpdateApplicationConfig(ApplicationConfigurationModel configurationModel)
    {
        var entity = this.applicationConfigurationRepository.Get(configurationModel.id);
        entity.Value = configurationModel.Value;
        entity.Description = configurationModel.Description;

        this.applicationConfigurationRepository.Update(entity);  
    }

    public string GetValueByName(string name)
    {
        
        return this.applicationConfigurationRepository.GetValueByName(name);
    }

    public ApplicationConfigurationModel GetById(string Id)
    {
        ApplicationConfigurationModel model = new ApplicationConfigurationModel();
        var config =this.applicationConfigurationRepository.Get(Id);
        model.id = config.id;
        model.Name = config.Name; 
        model.Value = config.Value;
        model.Description = config.Description;

        return model;
    }

    /// <summary>
    /// Updates the application log.
    /// </summary>
    /// <param name="logMessage">The log message.</param>
    public IEnumerable<ApplicationConfigurationModel> GetAllConfig()
    {
        var configs = this.applicationConfigurationRepository.GetAll().ToList();

        if(configs.Count==0)
        {
            this.AddInitialConfiguration();
            configs = this.applicationConfigurationRepository.GetAll().ToList();
        }



        var configModelList = new List<ApplicationConfigurationModel>();
        
        foreach (var config in configs)
        {
            var entity = new ApplicationConfigurationModel();
            entity.id = config.id;
            entity.Name = config.Name;
            entity.Value = config.Value;
            entity.Description = config.Description;
            configModelList.Add(entity);
        }

        return configModelList;
    }

    public void AddInitialConfiguration()
    {
        this.AddConfiguration("SMTPFromEmail", "", "SMTP FromEmail address");
        this.AddConfiguration("SMTPPassword", "", "SMTP Password");
        this.AddConfiguration("SMTPPort", "", "SMTP Port");
        this.AddConfiguration("SMTPHost", "", "SMTP Server address");
        this.AddConfiguration("SMTPUserName", "", "SMTP User name");
        this.AddConfiguration("SMTPSslEnabled", "", "Enable SSL");
        this.AddConfiguration("ApplicationName", "", "Application Name");
        this.AddConfiguration("SchedulerEmailTo", "", "SMTP FromEmail address");
        this.AddConfiguration("EnablesSuccessfulSchedulerEmail", "", "Enable Successful Email");
        this.AddConfiguration("EnablesFailureSchedulerEmail", "", "Enable Failure Email");
        this.AddConfiguration("EnablesMissingSchedulerEmail", "", "Enable Missing Email");
        this.AddConfiguration("Failure_Subject", "Scheduled Metered Task Failure!", "Failure Email Subject");
        this.AddConfiguration("Accepted_Subject", "Scheduled Metered Task Submitted Successfully!", "Successful Email Subject");
        this.AddConfiguration("Missing_Subject", "Scheduled Metered Task was Skipped!", "Missing Email Subject");
        this.AddConfiguration("Missing_Email", "<html><head></head><body><center><table align=center><tr><td><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was <b>skipped</b> by scheduler engine!</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/>****ResponseJson****</td></tr></table></center></body> </html>", "Missing Email Template");
        this.AddConfiguration("Accepted_Email", "<html><head></head><body><center><table align=center><tr><td><h2>Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired <b>Successfully</b></p><p>The following section is the deatil results.</p><hr/>****ResponseJson****</td></tr></table></center></body> </html>", "Successful Email Template");
        this.AddConfiguration("Failure_Email", "<html><head></head><body><center><table align=center><tr><td><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired<b> but Failed to Submit Data</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/>****ResponseJson****</td></tr></table></center></body> </html>", "Failure Email Template");

    }
    private void AddConfiguration(string name, string value, string description)
    {
        var entity = new ApplicationConfiguration();
        entity.id=Guid.NewGuid().ToString();
        entity.Name = name;
        entity.Value = value;
        entity.Description = description;
        entity.PartitionKey = entity.id;

        this.applicationConfigurationRepository.Save(entity);
    }
}