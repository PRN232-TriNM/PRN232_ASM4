using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Implements
{
    public class ChargerService : IChargerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChargerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<ChargerTriNM>> GetAllChargersAsync()
        {
            return await _unitOfWork.ChargerTriNMRepository.GetAllAsync();
        }

        public async Task<ChargerTriNM?> GetChargerByIdAsync(int id)
        {
            return await _unitOfWork.ChargerTriNMRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ChargerTriNM>> GetChargersByStationIdAsync(int stationId)
        {
            return await _unitOfWork.ChargerTriNMRepository.GetByStationTriNMIdAsync(stationId);
        }

        public async Task<IEnumerable<ChargerTriNM>> GetAvailableChargersAsync()
        {
            var allChargers = await _unitOfWork.ChargerTriNMRepository.GetAllAsync();
            return allChargers.Where(c => c.IsAvailable);
        }

        public async Task<ChargerTriNM> CreateChargerAsync(ChargerTriNM charger)
        {
            ArgumentNullException.ThrowIfNull(charger);

            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(charger.StationTriNMId);
            if (station == null)
            {
                throw new ArgumentException($"Station with ID {charger.StationTriNMId} not found");
            }

            await _unitOfWork.ChargerTriNMRepository.AddAsync(charger);
            await _unitOfWork.SaveChangesAsync();

            return charger;
        }

        public async Task<bool> UpdateChargerAsync(ChargerTriNM charger)
        {
            ArgumentNullException.ThrowIfNull(charger);

            var existingCharger = await _unitOfWork.ChargerTriNMRepository.GetByIdAsync(charger.ChargerTriNMId);
            if (existingCharger == null)
            {
                return false;
            }

            await _unitOfWork.ChargerTriNMRepository.UpdateAsync(charger);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteChargerAsync(int id)
        {
            var charger = await _unitOfWork.ChargerTriNMRepository.GetByIdAsync(id);
            if (charger == null)
            {
                return false;
            }

            await _unitOfWork.ChargerTriNMRepository.DeleteAsync(charger);
            return true;
        }
    }
}

