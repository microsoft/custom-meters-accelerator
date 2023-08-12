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
public class ApplicationLogService
{
    /// <summary>
    /// The application log repository.
    /// </summary>
    private readonly IApplicationLogRepository applicationLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLogService"/> class.
    /// </summary>
    /// <param name="applicationLogRepository">The application log repository.</param>
    public ApplicationLogService(IApplicationLogRepository applicationLogRepository)
    {
        this.applicationLogRepository = applicationLogRepository;
    }

    /// <summary>
    /// Adds the application log.
    /// </summary>
    /// <param name="logMessage">The log message.</param>
    public int AddApplicationLog(string logMessage)
    {
        var id = Guid.NewGuid().ToString();
        ApplicationLog newLog = new ApplicationLog()
        {
            id = id,
            ActionTime = DateTime.Now,
            LogDetail = HttpUtility.HtmlEncode(logMessage),
            PartitionKey=id
        };

        return this.applicationLogRepository.Save(newLog);
    }

    /// <summary>
    /// Updates the application log.
    /// </summary>
    /// <param name="logMessage">The log message.</param>
    public IEnumerable<ApplicationLogModel> GetAllLogs()
    {
        var logs = this.applicationLogRepository.GetAll();


        var logModelList = new List<ApplicationLogModel>();
        
        foreach (var log in logs)
        {
            var entity = new ApplicationLogModel();
            entity.id = log.id;
            entity.LogDetail = log.LogDetail;
            entity.ActionTime = log.ActionTime;
            logModelList.Add(entity);
        }

        return logModelList;
    }
}