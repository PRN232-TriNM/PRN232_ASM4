using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCS.TriNM.Repositories.Repository
{
    public class ChargerTriNMRepository : GenericRepository<ChargerTriNM>, IChargerTriNMRepository
    {
        public ChargerTriNMRepository(EVChargingDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChargerTriNM>> GetByStationTriNMIdAsync(int StationTriNMId)
        {
            return await _context.ChargerTriNMs
                .Where(c => c.StationTriNMId == StationTriNMId)
                .ToListAsync();
        }
    }

    public interface IChargerTriNMRepository : IGenericRepository<ChargerTriNM>
    {
        Task<IEnumerable<ChargerTriNM>> GetByStationTriNMIdAsync(int StationTriNMId);
    }
}
