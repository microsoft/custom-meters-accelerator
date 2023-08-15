using System.Collections.Generic;
using System.Threading.Tasks;
using ManagedApplicationScheduler.DataAccess.Entities;

namespace ManagedApplicationScheduler.DataAccess.Contracts
{
    /// <summary>
    /// Repository to access application log entries.
    /// </summary>
    public interface IApplicationConfigurationRepository
    {
        IEnumerable<ApplicationConfiguration> GetAll();
        ApplicationConfiguration? Get(string id);
        int Save(ApplicationConfiguration entity);
        void Update(ApplicationConfiguration entity);
        void Remove(ApplicationConfiguration entity);
        string? GetValueByName(string name);
    }
}