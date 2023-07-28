using ManagedApplicationScheduler.DataAccess.Entities;


namespace ManagedApplicationScheduler.DataAccess.Contracts
{
    public interface ISubscriptionsRepository
    {
        IEnumerable<Subscription> GetAll();
        Subscription Get(string id);
        int Save(Subscription entity);
        void Update(Subscription entity);
        void Remove(Subscription entity);
    }
}
