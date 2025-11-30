using EVCS.TriNM.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<StationTriNM>> GetAllStationsAsync(bool includeInactive = true);
        Task<StationTriNM?> GetStationByIdAsync(int id);
        Task<IEnumerable<StationTriNM>> GetActiveStationsAsync();
        Task<StationTriNM?> GetStationWithChargersAsync(int stationId);
        Task<IEnumerable<StationTriNM>> GetStationsByLocationAsync(string location);
        Task<IEnumerable<StationTriNM>> SearchStationsByCodeNameOrAddressAsync(string searchTerm);
        Task<IEnumerable<StationTriNM>> GetAvailableStationsAsync();
        Task<PagedResult<StationTriNM>> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize);
        Task<PagedResult<StationTriNM>> GetStationsPagedAsync(int pageNumber, int pageSize);
        Task<StationTriNM> CreateStationAsync(StationTriNM station);
        Task<StationTriNM> UpdateStationAsync(StationTriNM station);
        Task<bool> DeleteStationAsync(int id, bool hardDelete = false);
        Task<bool> ActivateStationAsync(int id);
        Task<bool> UpdateStationAvailabilityAsync(int stationId, int newAvailable);
    }
}

