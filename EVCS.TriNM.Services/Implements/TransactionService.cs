using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<ChargingTransaction>> GetAllTransactionsAsync()
        {
            return await _unitOfWork.ChargingTransactionRepository.GetAllAsync();
        }

        public async Task<ChargingTransaction?> GetTransactionByIdAsync(long id)
        {
            return await _unitOfWork.ChargingTransactionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ChargingTransaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _unitOfWork.ChargingTransactionRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<ChargingTransaction>> GetTransactionsByChargerIdAsync(int chargerId)
        {
            var allTransactions = await _unitOfWork.ChargingTransactionRepository.GetAllAsync();
            return allTransactions.Where(t => t.ChargerTriNMId == chargerId);
        }

        public async Task<ChargingTransaction> CreateTransactionAsync(ChargingTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            var charger = await _unitOfWork.ChargerTriNMRepository.GetByIdAsync(transaction.ChargerTriNMId);
            if (charger == null)
            {
                throw new ArgumentException($"Charger with ID {transaction.ChargerTriNMId} not found");
            }

            if (!charger.IsAvailable)
            {
                throw new InvalidOperationException($"Charger with ID {transaction.ChargerTriNMId} is not available");
            }

            var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(transaction.UserAccountId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {transaction.UserAccountId} not found");
            }

            transaction.StartTime = DateTime.UtcNow;
            transaction.CreatedDate = DateTime.UtcNow;
            transaction.PaymentStatus = "Pending";

            charger.IsAvailable = false;
            await _unitOfWork.ChargerTriNMRepository.UpdateAsync(charger);

            await _unitOfWork.ChargingTransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return transaction;
        }

        public async Task<bool> UpdateTransactionAsync(ChargingTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            var existingTransaction = await _unitOfWork.ChargingTransactionRepository.GetByIdAsync(transaction.TransactionId);
            if (existingTransaction == null)
            {
                return false;
            }

            await _unitOfWork.ChargingTransactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteTransactionAsync(long transactionId, decimal energyConsumed, decimal amount)
        {
            var transaction = await _unitOfWork.ChargingTransactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return false;
            }

            transaction.EndTime = DateTime.UtcNow;
            transaction.EnergyConsumed = energyConsumed;
            transaction.Amount = amount;
            transaction.PaymentStatus = "Completed";

            var charger = await _unitOfWork.ChargerTriNMRepository.GetByIdAsync(transaction.ChargerTriNMId);
            if (charger != null)
            {
                charger.IsAvailable = true;
                await _unitOfWork.ChargerTriNMRepository.UpdateAsync(charger);
            }

            await _unitOfWork.ChargingTransactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

