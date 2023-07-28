using Azure.Identity;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ManagedApplicationScheduler.MeteredTriggerJob;
public class Executor
{

    private readonly ManagedAppClientConfiguration config;
    private SchedulerService schedulerService;
    private UsageResultService usageResultService;

    public Executor(ISchedulerTasksRepository schedulerTasksRepository,
        IUsageResultRepository usageResultRepository, ManagedAppClientConfiguration config)
    {

        schedulerService = new SchedulerService(schedulerTasksRepository);
        usageResultService = new UsageResultService(usageResultRepository);
        this.config = config;
    }




    public async Task ExecuteAsync()
    {
        var creds = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
        var token = creds.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { config.Scope }), CancellationToken.None).Result.Token;

        //Get all Scheduled Data
        List<ScheduledTasksModel> getScheduledTasks = schedulerService.GetEnabledSchedulersTasks();

        if (getScheduledTasks.Count > 0)
        {

            //GetCurrentUTC time
            DateTime _currentUTCTime = DateTime.UtcNow;
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Hour, 0, 0);
            _currentUTCTime = _currentUTCTime.Date + ts;

            //Process each scheduler frequency
            foreach (SchedulerFrequencyEnum frequency in Enum.GetValues(typeof(SchedulerFrequencyEnum)))
            {

                Console.WriteLine();
                Console.WriteLine($"==== Checking all {frequency} scheduled items at {_currentUTCTime} UTC. ====");

                var scheduledItems = getScheduledTasks
                    .Where(a => a.Frequency == frequency.ToString())
                    .ToList();

                foreach (var scheduledItem in scheduledItems)
                {
                    // Get the run time.
                    //Always pickup the NextRuntime, durnig firstRun or OneTime then pickup StartDate, as the NextRunTime will be null
                    DateTime? _nextRunTime = scheduledItem.NextRunTime ?? scheduledItem.StartDate;
                    int timeDifferentInHours = (int)_currentUTCTime.Subtract(_nextRunTime.Value).TotalHours;

                    // Print the scheduled Item and the expected run date
                    PrintScheduler(scheduledItem,
                        _nextRunTime,
                        timeDifferentInHours);


                    if (timeDifferentInHours < 0)
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.ScheduledTaskName} future run will be at {_nextRunTime} UTC.");
                        continue;
                    }
                    else if (timeDifferentInHours >= 0)// if it is in the past and still mark as scheduled, engine will attempt to run it
                    {
                        await TriggerSchedulerItemAsync(scheduledItem, token, config.Marketplace_Uri);

                    }
                    else
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.ScheduledTaskName} will not run as it doesn't match any time difference logic. {_nextRunTime} UTC.");
                    }



                }
            }

            Console.WriteLine("==== Completed processing all Waiting Scheduled Tasks to process! ====");


        }
        else
        {
            Console.WriteLine("==== Could not find any Waiting Scheduled Tasks to process! ==== ");
        }

    }

    public async Task TriggerSchedulerItemAsync(ScheduledTasksModel item,
        string token,
        string marketplaceUrl)
    {
        try
        {
            Console.WriteLine($"---- Item Id: {item.ScheduledTaskName} Start Triggering meter event ----");

            var subscriptionUsageRequest = new MeteredUsageRequestModel()
            {
                Dimension = item.Dimension,
                EffectiveStartTime = DateTime.UtcNow,
                PlanId = item.PlanId,
                Quantity = item.Quantity,
                ResourceUri = item.ResourceUri,
            };
            var meteringUsageResult = new MeteredUsageResultModel();
            var responseBody = "";
            var requestJson = JsonSerializer.Serialize(subscriptionUsageRequest);
            try
            {

                Console.WriteLine($"Item Id: {item.ScheduledTaskName} Request {requestJson}");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = httpClient.PostAsJsonAsync(marketplaceUrl, subscriptionUsageRequest).Result;

                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                if (responseBody.Contains("additionalInfo"))
                {
                    var errorResult = JsonSerializer.Deserialize<MeteredUsageErrorResultModel>(responseBody);
                    meteringUsageResult = errorResult.additionalInfo.acceptedMessage;
                    meteringUsageResult.Message = errorResult.Message;
                }
                else
                {
                    meteringUsageResult = JsonSerializer.Deserialize<MeteredUsageResultModel>(responseBody);
                }


                Console.WriteLine($"Item Id: {item.ScheduledTaskName} Response {responseBody}");

            }
            catch (Exception marketplaceException)
            {
                responseBody = JsonSerializer.Serialize(marketplaceException.Message);
                meteringUsageResult.Status = SchedulerStatusEnum.Error.ToString();
                Console.WriteLine($" Item Id: {item.ScheduledTaskName} Error during EmitUsageEventAsync {responseBody}");
            }

            meteringUsageResult.ScheduledTaskName = item.ScheduledTaskName;
            UpdateSchedulerItem(item,
                meteringUsageResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
    public void UpdateSchedulerItem(ScheduledTasksModel item, MeteredUsageResultModel meteringUsageResult)
    {
        try
        {
            Console.WriteLine($"Item Id: {item.ScheduledTaskName} Saving Audit information");
            Console.WriteLine($"Item Id: {item.ScheduledTaskName} current Status is {meteringUsageResult.Status}");

            // Was it Successful post
            if ((meteringUsageResult.Status == "Accepted"))
            {
                meteringUsageResult.Message = "Metered submission is accepted.";
                Console.WriteLine($"Item Id: {item.ScheduledTaskName} Meter event Accepted");

                //Ignore updating NextRuntime value for OneTime frequency as they always depend on StartTime value
                Enum.TryParse(item.Frequency, out SchedulerFrequencyEnum itemFrequency);
                if (itemFrequency != SchedulerFrequencyEnum.OneTime)
                {
                    var _nextRunTime = GetNextRunTime(item.NextRunTime ?? item.StartDate, itemFrequency);
                    Console.WriteLine($"Item Id: {item.ScheduledTaskName} Updating Scheduler NextRunTime from {item.NextRunTime} to {_nextRunTime}");
                    item.NextRunTime = _nextRunTime;
                }
                else
                {
                    item.Status = SchedulerStatusEnum.Completed.ToString();
                }
            }
            else
            {
                //Update Task Status
                item.Status = SchedulerStatusEnum.Error.ToString();
            }

            //Save the Result to database
            usageResultService.SaveUsageResult(meteringUsageResult);

            //Update the Scheduled Task
            schedulerService.UpdateScheduler(item);
            Console.WriteLine($"Item Id: {item.ScheduledTaskName} Complete Triggering Meter event.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void PrintScheduler(ScheduledTasksModel item,
        DateTime? nextRun,
        int timeDifferenceInHours)
    {
        Console.WriteLine($"Item Id: {item.id} " +
                          $"Expected NextRun : {nextRun} " +
                          $"ResourceUri : {item.ResourceUri} " +
                          $"Plan : {item.PlanId} " +
                          $"Dim : {item.Dimension} " +
                          $"Start Date : {item.StartDate} " +
                          $"NextRun : {item.NextRunTime}" +
                          $"TimeDifferenceInHours : {timeDifferenceInHours}");
    }
    public DateTime? GetNextRunTime(DateTime? startDate, SchedulerFrequencyEnum frequency)
    {
        switch (frequency)
        {
            case SchedulerFrequencyEnum.Hourly: { return startDate.Value.AddHours(1); }
            case SchedulerFrequencyEnum.Daily: { return startDate.Value.AddDays(1); }
            case SchedulerFrequencyEnum.Weekly: { return startDate.Value.AddDays(7); }
            case SchedulerFrequencyEnum.Monthly: { return startDate.Value.AddMonths(1); }
            case SchedulerFrequencyEnum.Yearly: { return startDate.Value.AddYears(1); }
            case SchedulerFrequencyEnum.OneTime: { return startDate; }
            default:
                { return null; }
        }
    }
}