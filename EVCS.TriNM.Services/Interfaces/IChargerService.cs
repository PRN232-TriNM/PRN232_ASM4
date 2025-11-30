using EVCS.TriNM.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IChargerService
    {
        Task<IEnumerable<ChargerTriNM>> GetAllChargersAsync();
        Task<ChargerTriNM?> GetChargerByIdAsync(int id);
        Task<IEnumerable<ChargerTriNM>> GetChargersByStationIdAsync(int stationId);
        Task<IEnumerable<ChargerTriNM>> GetAvailableChargersAsync();
        Task<ChargerTriNM> CreateChargerAsync(ChargerTriNM charger);
        Task<bool> UpdateChargerAsync(ChargerTriNM charger);
        Task<bool> DeleteChargerAsync(int id);
    }
}

