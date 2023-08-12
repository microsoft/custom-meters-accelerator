using ManagedApplicationScheduler.DataAccess.Context;
using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Services
{
    public class SubscriptionsRepository : ISubscriptionsRepository
    {

        /// <summary>
        /// The context.
        /// </summary>
        private readonly CosmosDbContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;


        public SubscriptionsRepository(CosmosDbContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreated();


        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<Subscription> GetAll()
        {
            return this.context.Subscription.ToList();

            //Where(s => s.id.StartsWith("|")).ToList();
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Subscription Get(string id)
        {
            return context.Subscription.Where(s => s.id == id).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(Subscription entity)
        {

            this.context.Add(entity);
            return this.context.SaveChanges();


        }

        public void Update(Subscription entity)
        {
            this.context.Update(entity);
            this.context.SaveChanges();
        }
        public void Remove(Subscription entity)
        {
            this.context.Remove(entity);
            this.context.SaveChanges();
        }


    }
}

