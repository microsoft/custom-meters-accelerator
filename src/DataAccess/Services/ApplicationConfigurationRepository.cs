using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Services
{

    public class ApplicationConfigurationRepository : IApplicationConfigurationRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly CosmosDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ApplicationConfigurationRepository(CosmosDbContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreated();
        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<ApplicationConfiguration> GetAll()
        {
            return this.context.ApplicationConfiguration;
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApplicationConfiguration? Get(string id)
        {
            return this.context.ApplicationConfiguration.Where(s => s.id == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(ApplicationConfiguration entity)
        {
            this.context.Add(entity);
            return this.context.SaveChanges();

        }

        public string? GetValueByName(string name)
        {

            var config = context.ApplicationConfiguration.Where(s => s.Name == name).FirstOrDefault();
            return config?.Value;
        }

        public void Update(ApplicationConfiguration entity)
        {
            this.context.Update(entity);
            this.context.SaveChanges();
        }

        public void Remove(ApplicationConfiguration entity)
        {
            this.context.Remove(entity);
            this.context.SaveChanges();
        }


    }


}
