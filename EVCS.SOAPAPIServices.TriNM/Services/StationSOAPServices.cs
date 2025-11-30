using EVCS.SOAPAPIServices.TriNM.SOAPModels;
using EVCS.TriNM.Services.Implements;
using EVCS.TriNM.Services.Interfaces;
using EVCS.SOAPAPIServices.TriNM.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.ServiceModel;

namespace EVCS.SOAPAPIServices.TriNM.Services
{
    [ServiceContract]
    public interface IStationSOAPServices
    {
        //Query
        [OperationContract]
        Task<List<Station>> GetAllAsync();

        [OperationContract]
        Task<Station?> GetAsync(int id);

        [OperationContract]
        Task<List<Station>> SearchStationsAsync(string searchTerm);

        [OperationContract]
        Task<List<Station>> GetAvailableStationsAsync();

        [OperationContract]
        Task<PagedStationResult> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize);

        [OperationContract]
        Task<PagedStationResult> GetStationsPagedAsync(int pageNumber, int pageSize);

        //Mutation
        [OperationContract]
        Task<int> CreateStationAsync(Station station);

        [OperationContract]
        Task<int> UpdateStationAsync(Station station);

        [OperationContract]
        Task<int> DeleteStationAsync(int id);

        [OperationContract]
        Task<bool> ActivateStationAsync(int id);

        [OperationContract]
        Task<bool> UpdateAvailabilityAsync(int stationId, int newAvailable);
    }

    public class StationSOAPServices : IStationSOAPServices
    {
        private readonly IServiceProviders _servicesProvider;
        private readonly IHubContext<StationHub> _hubContext;
        private IStationService StationService => _servicesProvider.StationService;

        public StationSOAPServices(IServiceProviders servicesProvider, IHubContext<StationHub> hubContext)
        {
            _servicesProvider = servicesProvider ?? throw new ArgumentNullException(nameof(servicesProvider));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<List<Station>> GetAllAsync()
        {
            try
            {
                var stations = await StationService.GetAllStationsAsync(includeInactive: true);
                return stations.Select(MapToSOAPModel).ToList();
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error retrieving stations: {ex.Message}");
            }
        }

        public async Task<Station?> GetAsync(int id)
        {
            try
            {
                var station = await StationService.GetStationByIdAsync(id);
                return station != null ? MapToSOAPModel(station) : null;
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error retrieving station with ID {id}: {ex.Message}");
            }
        }

        public async Task<List<Station>> SearchStationsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllAsync();

                var stations = await StationService.SearchStationsByCodeNameOrAddressAsync(searchTerm);
                return stations.Select(MapToSOAPModel).ToList();
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error searching stations: {ex.Message}");
            }
        }

        public async Task<List<Station>> GetAvailableStationsAsync()
        {
            try
            {
                var stations = await StationService.GetAvailableStationsAsync();
                return stations.Select(MapToSOAPModel).ToList();
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error retrieving available stations: {ex.Message}");
            }
        }

        public async Task<PagedStationResult> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize)
        {
            try
            {
                var result = await StationService.SearchStationsPagedAsync(name, location, isActive, pageNumber, pageSize);
                return new PagedStationResult
                {
                    Items = result.Items.Select(MapToSOAPModel).ToList(),
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasPreviousPage = result.HasPreviousPage,
                    HasNextPage = result.HasNextPage
                };
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error searching stations: {ex.Message}");
            }
        }

        public async Task<PagedStationResult> GetStationsPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var result = await StationService.GetStationsPagedAsync(pageNumber, pageSize);
                return new PagedStationResult
                {
                    Items = result.Items.Select(MapToSOAPModel).ToList(),
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasPreviousPage = result.HasPreviousPage,
                    HasNextPage = result.HasNextPage
                };
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error retrieving stations: {ex.Message}");
            }
        }

        public async Task<int> CreateStationAsync(Station station)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(station);

                // Validate required fields
                ValidateStation(station);

                var repoStation = MapToRepositoryModel(station);
                var createdStation = await StationService.CreateStationAsync(repoStation);
                
                try
                {
                    await _hubContext.Clients.Group("Stations").SendAsync("StationCreated", MapToSOAPModel(createdStation));
                    Console.WriteLine($"SignalR: StationCreated event sent for station ID: {createdStation.StationTriNMId}");
                }
                catch (Exception hubEx)
                {
                    Console.WriteLine($"SignalR notification error for StationCreated: {hubEx.Message}");
                }
                
                return createdStation.StationTriNMId;
            }
            catch (ArgumentException ex)
            {
                throw new FaultException($"Validation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                throw new FaultException($"Business rule violation: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error creating station: {ex.Message}");
            }
        }

        public async Task<int> UpdateStationAsync(Station station)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(station);

                // Validate required fields
                ValidateStation(station);

                var repoStation = MapToRepositoryModel(station);
                var updatedStation = await StationService.UpdateStationAsync(repoStation);
                
                try
                {
                    await _hubContext.Clients.Group("Stations").SendAsync("StationUpdated", MapToSOAPModel(updatedStation));
                    Console.WriteLine($"SignalR: StationUpdated event sent for station ID: {updatedStation.StationTriNMId}");
                }
                catch (Exception hubEx)
                {
                    Console.WriteLine($"SignalR notification error for StationUpdated: {hubEx.Message}");
                }
                
                return updatedStation.StationTriNMId;
            }
            catch (ArgumentException ex)
            {
                throw new FaultException($"Validation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                throw new FaultException($"Business rule violation: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error updating station: {ex.Message}");
            }
        }

        public async Task<int> DeleteStationAsync(int id)
        {
            try
            {
                var success = await StationService.DeleteStationAsync(id, hardDelete: true);
                if (success)
                {
                    try
                    {
                        await _hubContext.Clients.Group("Stations").SendAsync("StationDeleted", id);
                        Console.WriteLine($"SignalR: StationDeleted event sent for station ID: {id}");
                    }
                    catch (Exception hubEx)
                    {
                        Console.WriteLine($"SignalR notification error for StationDeleted: {hubEx.Message}");
                    }
                }
                return success ? 1 : 0;
            }
            catch (InvalidOperationException ex)
            {
                throw new FaultException($"Cannot delete station: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error deleting station: {ex.Message}");
            }
        }

        public async Task<bool> ActivateStationAsync(int id)
        {
            try
            {
                return await StationService.ActivateStationAsync(id);
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error activating station: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAvailabilityAsync(int stationId, int newAvailable)
        {
            try
            {
                return await StationService.UpdateStationAvailabilityAsync(stationId, newAvailable);
            }
            catch (ArgumentException ex)
            {
                throw new FaultException($"Invalid availability value: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error updating availability: {ex.Message}");
            }
        }

        #region Helper Methods

        /// <summary>
        /// Map Repository Station model to SOAP Station model
        /// </summary>
        private static Station MapToSOAPModel(EVCS.TriNM.Repositories.Models.StationTriNM repoStation)
        {
            return new Station
            {
                StationId = repoStation.StationTriNMId,
                StationCode = repoStation.StationTriNMCode,
                StationName = repoStation.StationTriNMName,
                Address = repoStation.Address,
                City = repoStation.City,
                Province = repoStation.Province,
                Latitude = repoStation.Latitude,
                Longitude = repoStation.Longitude,
                Capacity = repoStation.Capacity,
                CurrentAvailable = repoStation.CurrentAvailable,
                Owner = repoStation.Owner,
                ContactPhone = repoStation.ContactPhone,
                ContactEmail = repoStation.ContactEmail,
                Description = repoStation.Description,
                CreatedDate = repoStation.CreatedDate,
                ModifiedDate = repoStation.ModifiedDate,
                IsActive = repoStation.IsActive,
                Chargers = []
            };
        }

        private static EVCS.TriNM.Repositories.Models.StationTriNM MapToRepositoryModel(Station soapStation)
        {
            return new EVCS.TriNM.Repositories.Models.StationTriNM
            {
                StationTriNMId = soapStation.StationId,
                StationTriNMCode = soapStation.StationCode,
                StationTriNMName = soapStation.StationName,
                Address = soapStation.Address,
                City = soapStation.City,
                Province = soapStation.Province,
                Latitude = soapStation.Latitude,
                Longitude = soapStation.Longitude,
                Capacity = soapStation.Capacity,
                CurrentAvailable = soapStation.CurrentAvailable,
                Owner = soapStation.Owner,
                ContactPhone = soapStation.ContactPhone,
                ContactEmail = soapStation.ContactEmail,
                Description = soapStation.Description,
                CreatedDate = soapStation.CreatedDate,
                ModifiedDate = soapStation.ModifiedDate,
                IsActive = soapStation.IsActive
            };
        }

        /// <summary>
        /// Validate Station data
        /// </summary>
        private static void ValidateStation(Station station)
        {
            if (string.IsNullOrWhiteSpace(station.StationCode))
                throw new ArgumentException("Station Code is required");

            if (string.IsNullOrWhiteSpace(station.StationName))
                throw new ArgumentException("Station Name is required");

            if (string.IsNullOrWhiteSpace(station.Address))
                throw new ArgumentException("Address is required");

            if (string.IsNullOrWhiteSpace(station.Owner))
                throw new ArgumentException("Owner is required");

            if (station.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0");

            if (station.CurrentAvailable < 0 || station.CurrentAvailable > station.Capacity)
                throw new ArgumentException("Current Available must be between 0 and Capacity");

            // Validate coordinates if provided
            if (station.Latitude.HasValue && (station.Latitude < -90 || station.Latitude > 90))
                throw new ArgumentException("Latitude must be between -90 and 90");

            if (station.Longitude.HasValue && (station.Longitude < -180 || station.Longitude > 180))
                throw new ArgumentException("Longitude must be between -180 and 180");
        }

        #endregion
    }
}
