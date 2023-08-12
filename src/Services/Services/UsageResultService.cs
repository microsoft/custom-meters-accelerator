using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;
using ManagedApplicationScheduler.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedApplicationScheduler.Services.Services
{
    public class UsageResultService
    {
        private IUsageResultRepository usageResultRepository;

        public UsageResultService(IUsageResultRepository usageResultRepository)
        {
            this.usageResultRepository = usageResultRepository;
        }

        public int SaveUsageResult(MeteredUsageResultModel meteringUsageResultModel)
        {

            var entity = new UsageResult();
            entity.id = Guid.NewGuid().ToString();
            entity.Message = meteringUsageResultModel.Message;
            entity.UsagePostedDate = meteringUsageResultModel.UsagePostedDate;
            entity.Quantity = meteringUsageResultModel.Quantity;
            entity.Status = meteringUsageResultModel.Status;
            entity.UsageEventId = meteringUsageResultModel.UsageEventId;
            entity.MessageTime = meteringUsageResultModel.MessageTime;
            entity.ResourceId = meteringUsageResultModel.ResourceId;
            entity.Dimension = meteringUsageResultModel.Dimension;
            entity.PlanId = meteringUsageResultModel.PlanId;
            entity.ScheduledTaskName = meteringUsageResultModel.ScheduledTaskName;
            entity.PartitionKey = entity.id;
            entity.ResourceUri = meteringUsageResultModel.ResourceUri;
            return this.usageResultRepository.Save(entity);
        }

        public List<MeteredUsageResultModel> GetAllUsages()
        {
            var usages = new List<MeteredUsageResultModel>();
            var entities = this.usageResultRepository.GetAll();
            foreach (UsageResult entity in entities)
            {
                var usage = new MeteredUsageResultModel();
                usage.id = entity.id;
                usage.Message = entity.Message;
                usage.UsagePostedDate = entity.UsagePostedDate;
                usage.Quantity = entity.Quantity;
                usage.Status = entity.Status;
                usage.UsageEventId = entity.UsageEventId;
                usage.MessageTime = entity.MessageTime;
                usage.ResourceId = entity.ResourceId;
                usage.Dimension = entity.Dimension;
                usage.PlanId = entity.PlanId;
                usage.ResourceUri = entity.ResourceUri;
                usage.ScheduledTaskName = entity.ScheduledTaskName;
                usages.Add(usage);
            }

            return usages;
        }

        public List<MeteredUsageResultModel> GetUsageByTaskName(string scheduledTaskName, string resourceUri)
        {
            var usageList = this.GetAllUsages();

            var usages = usageList.Where(s => s.ScheduledTaskName == scheduledTaskName && s.ResourceUri == resourceUri).ToList();

            return usages;
        }


        public List<MeteredUsageResultModel> GetUsageBySubscription(string resourceUri)
        {
            var usageList = this.GetAllUsages();

            var usages = usageList.Where(s => s.ResourceUri == resourceUri).ToList();

            return usages;

        }


    }
}
