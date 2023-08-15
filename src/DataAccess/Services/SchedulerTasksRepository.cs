using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Services
{
    public class SchedulerTasksRepository : ISchedulerTasksRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly CosmosDbContext context;




        public SchedulerTasksRepository(CosmosDbContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreated();
        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<ScheduledTasks> GetAll()
        {
            return this.context.ScheduledTasks;
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScheduledTasks? Get(string id)
        {
            return this.context.ScheduledTasks.Where(s => s.id == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(ScheduledTasks entity)
        {
            this.context.Add(entity);
            return this.context.SaveChanges();

        }

        public void Update(ScheduledTasks entity)
        {

            this.context.Update(entity);
            this.context.SaveChanges();


        }

        public void Remove(ScheduledTasks entity)
        {
            this.context.Remove(entity);
            this.context.SaveChanges();
        }
    }
}
