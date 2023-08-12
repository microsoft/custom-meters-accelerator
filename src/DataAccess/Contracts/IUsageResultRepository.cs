using ManagedApplicationScheduler.DataAccess.Entities;


namespace ManagedApplicationScheduler.DataAccess.Contracts
{
    public interface IUsageResultRepository
    {
        IEnumerable<UsageResult> GetAll();
        UsageResult Get(string id);
        int Save(UsageResult entity);
    }
}
