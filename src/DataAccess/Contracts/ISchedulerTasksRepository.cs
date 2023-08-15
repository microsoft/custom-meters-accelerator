using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Contracts
{
    public interface ISchedulerTasksRepository
    {
        IEnumerable<ScheduledTasks> GetAll();
        ScheduledTasks? Get(string id);
        int Save(ScheduledTasks entity);
        void Update(ScheduledTasks entity);

        void Remove(ScheduledTasks entity);
    }
}
