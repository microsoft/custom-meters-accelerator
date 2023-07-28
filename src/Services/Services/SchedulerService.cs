using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;
using ManagedApplicationScheduler.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace ManagedApplicationScheduler.Services.Services
{
    public class SchedulerService
    {
        private ISchedulerTasksRepository schedulerRepository;
        public SchedulerService(ISchedulerTasksRepository schedulerRepository)
        {
            this.schedulerRepository = schedulerRepository;

        }
        public int SaveScheduler(ScheduledTasksModel task)
        {
            var entity = new ScheduledTasks();
            entity.id = task.id;
            entity.ScheduledTaskName = task.ScheduledTaskName;
            entity.StartDate = task.StartDate;
            entity.NextRunTime = task.NextRunTime;
            entity.PlanId = task.PlanId;
            entity.Dimension = task.Dimension;
            entity.Quantity = task.Quantity;
            entity.ResourceUri = task.ResourceUri;
            entity.PartitionKey = task.id;
            entity.Status = task.Status;
            entity.Frequency = task.Frequency;

            return this.schedulerRepository.Save(entity);

        }

        public void UpdateScheduler(ScheduledTasksModel task)
        {
            var entity = this.schedulerRepository.Get(task.id);
            entity.ScheduledTaskName = task.ScheduledTaskName;
            entity.StartDate = task.StartDate;
            entity.NextRunTime = task.NextRunTime;
            entity.PlanId = task.PlanId;
            entity.Dimension = task.Dimension;
            entity.Quantity = task.Quantity;
            entity.ResourceUri = task.ResourceUri;
            entity.PartitionKey = task.id;
            entity.Status = task.Status;
            entity.Frequency = task.Frequency;
            this.schedulerRepository.Update(entity);

        }

        public void DeleteScheduler(string id)
        {

            var entity = this.schedulerRepository.Get(id);
            this.schedulerRepository.Remove(entity);

        }



        public ScheduledTasksModel GetSchedulerByID(string id)
        {
            var entity = new ScheduledTasksModel();
            var task = this.schedulerRepository.Get(id);
            entity.id = task.id;
            entity.ScheduledTaskName = task.ScheduledTaskName;
            entity.StartDate = task.StartDate;
            entity.NextRunTime = task.NextRunTime;
            entity.PlanId = task.PlanId;
            entity.Dimension = task.Dimension;
            entity.Quantity = task.Quantity;
            entity.ResourceUri = task.ResourceUri;
            entity.PartitionKey = task.id;
            entity.Status = task.Status;
            entity.Frequency = task.Frequency;

            return entity;
        }

        public List<ScheduledTasksModel> GetAllSchedulersTasks()
        {
            var schedulers = new List<ScheduledTasksModel>();
            var tasks = this.schedulerRepository.GetAll();
            foreach (var task in tasks)
            {
                var entity = new ScheduledTasksModel();
                entity.id = task.id;
                entity.ScheduledTaskName = task.ScheduledTaskName;
                entity.StartDate = task.StartDate;
                entity.NextRunTime = task.NextRunTime;
                entity.PlanId = task.PlanId;
                entity.Dimension = task.Dimension;
                entity.Quantity = task.Quantity;
                entity.ResourceUri = task.ResourceUri;
                entity.PartitionKey = task.id;
                entity.Status = task.Status;
                entity.Frequency = task.Frequency;
                schedulers.Add(entity);
            }

            return schedulers;
        }

        public List<ScheduledTasksModel> GetSchedulersTasksBySubscription(string resourceUri)
        {
            var schedulers = new List<ScheduledTasksModel>();
            var tasks = this.schedulerRepository.GetAll().Where(s => s.ResourceUri == resourceUri).ToList();


            foreach (var task in tasks)
            {
                var entity = new ScheduledTasksModel();
                entity.id = task.id;
                entity.ScheduledTaskName = task.ScheduledTaskName;
                entity.StartDate = task.StartDate;
                entity.NextRunTime = task.NextRunTime;
                entity.PlanId = task.PlanId;
                entity.Dimension = task.Dimension;
                entity.Quantity = task.Quantity;
                entity.ResourceUri = task.ResourceUri;
                entity.PartitionKey = task.id;
                entity.Status = task.Status;
                entity.Frequency = task.Frequency;
                schedulers.Add(entity);
            }

            return schedulers;
        }

        public List<ScheduledTasksModel> GetEnabledSchedulersTasks()
        {
            var schedulers = GetAllSchedulersTasks();

            var enabledTasks = schedulers.Where(s => s.Status == SchedulerStatusEnum.Scheduled.ToString()).ToList();

            return enabledTasks;
        }

    }
}
