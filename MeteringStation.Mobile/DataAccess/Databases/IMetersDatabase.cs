using System.Collections.Generic;
using System.Threading.Tasks;
using MeteringStation.Mobile.DataAccess.Models;

namespace MeteringStation.Mobile.DataAccess.Databases
{
    public interface IMetersDatabase
    {
        Task<IEnumerable<RegisteredMeter>> GetMetersAsync();
        Task<int> RemoveMeterAsync(RegisteredMeter registeredMeter);
        Task UpsertAsync(RegisteredMeter registeredMeter);
    }
}