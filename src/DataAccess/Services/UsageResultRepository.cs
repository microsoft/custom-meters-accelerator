using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Services
{
    public class UsageResultRepository : IUsageResultRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly CosmosDbContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;


        public UsageResultRepository(CosmosDbContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreated();

        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<UsageResult> GetAll()
        {
            return this.context.UsageResult;
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UsageResult Get(string id)
        {
            return this.context.UsageResult.Where(s => s.id == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(UsageResult entity)
        {
            this.context.Add(entity);
            return this.context.SaveChanges();

        }
    }
}
