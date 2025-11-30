using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Implements
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<StationTriNM>> GetAllStationsAsync(bool includeInactive = true)
        {
            if (includeInactive)
            {
                return await _unitOfWork.StationTriNMRepository.GetAllAsync();
            }
            else
            {
                return await _unitOfWork.StationTriNMRepository.GetActiveStationTriNMsAsync();
            }
        }

        public async Task<StationTriNM?> GetStationByIdAsync(int id)
        {
            return await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<StationTriNM>> GetActiveStationsAsync()
        {
            return await _unitOfWork.StationTriNMRepository.GetActiveStationTriNMsAsync();
        }

        public async Task<StationTriNM?> GetStationWithChargersAsync(int stationId)
        {
            return await _unitOfWork.StationTriNMRepository.GetStationTriNMWithChargerTriNMsAsync(stationId);
        }

        public async Task<IEnumerable<StationTriNM>> GetStationsByLocationAsync(string location)
        {
            return await _unitOfWork.StationTriNMRepository.GetStationTriNMsByLocationAsync(location);
        }

        public async Task<IEnumerable<StationTriNM>> SearchStationsByCodeNameOrAddressAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllStationsAsync();
            }

            var allStations = await _unitOfWork.StationTriNMRepository.GetAllAsync();
            return allStations.Where(s => 
                (s.StationTriNMCode != null && s.StationTriNMCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (s.StationTriNMName != null && s.StationTriNMName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (s.Address != null && s.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }

        public async Task<IEnumerable<StationTriNM>> GetAvailableStationsAsync()
        {
            var allStations = await _unitOfWork.StationTriNMRepository.GetAllAsync();
            return allStations.Where(s => s.IsActive && s.CurrentAvailable > 0);
        }

        public async Task<PagedResult<StationTriNM>> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize)
        {
            return await _unitOfWork.StationTriNMRepository.SearchStationsPagedAsync(name, location, isActive, pageNumber, pageSize);
        }

        public async Task<PagedResult<StationTriNM>> GetStationsPagedAsync(int pageNumber, int pageSize)
        {
            return await _unitOfWork.StationTriNMRepository.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<StationTriNM> CreateStationAsync(StationTriNM station)
        {
            ArgumentNullException.ThrowIfNull(station);

            var allStations = await _unitOfWork.StationTriNMRepository.GetAllAsync();
            var existingStation = allStations.FirstOrDefault(s => s.StationTriNMCode == station.StationTriNMCode);
            if (existingStation != null)
            {
                throw new InvalidOperationException($"Station with code '{station.StationTriNMCode}' already exists.");
            }

            station.CreatedDate = DateTime.UtcNow;
            station.IsActive = true;
            if (station.CurrentAvailable == 0 && station.Capacity > 0)
            {
                station.CurrentAvailable = station.Capacity;
            }

            await _unitOfWork.StationTriNMRepository.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return station;
        }

        public async Task<StationTriNM> UpdateStationAsync(StationTriNM station)
        {
            ArgumentNullException.ThrowIfNull(station);

            var existingStation = await _unitOfWork.StationTriNMRepository.GetByIdAsync(station.StationTriNMId);
            if (existingStation == null)
            {
                throw new InvalidOperationException($"Station with ID {station.StationTriNMId} not found.");
            }

            if (existingStation.StationTriNMCode != station.StationTriNMCode)
            {
                var allStations = await _unitOfWork.StationTriNMRepository.GetAllAsync();
                var duplicateStation = allStations.FirstOrDefault(s => s.StationTriNMCode == station.StationTriNMCode);
                if (duplicateStation != null && duplicateStation.StationTriNMId != station.StationTriNMId)
                {
                    throw new InvalidOperationException($"Station with code '{station.StationTriNMCode}' already exists.");
                }
            }

            station.ModifiedDate = DateTime.UtcNow;
            await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return station;
        }

        public async Task<bool> DeleteStationAsync(int id, bool hardDelete = true)
        {
            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
            if (station == null)
            {
                return false;
            }

            var stationWithChargers = await _unitOfWork.StationTriNMRepository.GetStationTriNMWithChargerTriNMsAsync(id);
            if (stationWithChargers != null && stationWithChargers.ChargerTriNMs != null && stationWithChargers.ChargerTriNMs.Any())
            {
                throw new InvalidOperationException("Cannot delete station with existing chargers. Please remove chargers first.");
            }

            if (hardDelete)
            {
                await _unitOfWork.StationTriNMRepository.DeleteAsync(station);
            }
            else
            {
                station.IsActive = false;
                station.ModifiedDate = DateTime.UtcNow;
                await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateStationAsync(int id)
        {
            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
            if (station == null)
            {
                return false;
            }

            station.IsActive = true;
            station.ModifiedDate = DateTime.UtcNow;
            await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStationAvailabilityAsync(int stationId, int newAvailable)
        {
            if (newAvailable < 0)
            {
                throw new ArgumentException("Available count cannot be negative.");
            }

            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(stationId);
            if (station == null)
            {
                return false;
            }

            if (newAvailable > station.Capacity)
            {
                throw new ArgumentException($"Available count ({newAvailable}) cannot exceed capacity ({station.Capacity}).");
            }

            station.CurrentAvailable = newAvailable;
            station.ModifiedDate = DateTime.UtcNow;
            await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

