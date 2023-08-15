using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Services
{

    public class ApplicationLogRepository : IApplicationLogRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly CosmosDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ApplicationLogRepository(CosmosDbContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreated();
        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<ApplicationLog> GetAll()
        {
            return this.context.ApplicationLog;
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApplicationLog? Get(string id)
        {
            return this.context.ApplicationLog.Where(s => s.id == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(ApplicationLog entity)
        {
            this.context.Add(entity);
            return this.context.SaveChanges();

        }

        public void Update(ApplicationLog entity)
        {
            this.context.Update(entity);
            this.context.SaveChanges();
        }

        public void Remove(ApplicationLog entity)
        {
            this.context.Remove(entity);
            this.context.SaveChanges();
        }
    }


}
