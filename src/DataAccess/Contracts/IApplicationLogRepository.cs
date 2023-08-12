using System.Collections.Generic;
using System.Threading.Tasks;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Contracts
{
    /// <summary>
    /// Repository to access application log entries.
    /// </summary>
    public interface IApplicationLogRepository
    {
        IEnumerable<ApplicationLog> GetAll();
        ApplicationLog Get(string id);
        int Save(ApplicationLog entity);
        void Update(ApplicationLog entity);
        void Remove(ApplicationLog entity);

    }
}