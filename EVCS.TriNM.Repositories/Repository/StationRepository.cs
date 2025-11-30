using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Repositories.Repository
{
    public class StationTriNMRepository : GenericRepository<StationTriNM>
    {
        public StationTriNMRepository(EVChargingDBContext context) : base(context)
        {
        }

        public new async Task<PagedResult<StationTriNM>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<StationTriNM>()
                .OrderBy(s => s.StationTriNMId);
            return await GetPagedResultAsync(query, pageNumber, pageSize);
        }

        public new async Task<PagedResult<StationTriNM>> GetPagedAsync(System.Linq.Expressions.Expression<Func<StationTriNM, bool>> predicate, int pageNumber, int pageSize)
        {
            var query = _context.Set<StationTriNM>()
                .Where(predicate)
                .OrderBy(s => s.StationTriNMId);
            return await GetPagedResultAsync(query, pageNumber, pageSize);
        }

        public async Task<List<StationTriNM>> GetActiveStationTriNMsAsync()
        {
            return await _context.StationTriNMs
                .Where(s => s.IsActive == true)
                .Include(s => s.ChargerTriNMs)
                .ToListAsync();
        }

        public async Task<StationTriNM?> GetStationTriNMWithChargerTriNMsAsync(int StationTriNMId)
        {
            return await _context.StationTriNMs
                .Include(s => s.ChargerTriNMs)
                .FirstOrDefaultAsync(s => s.StationTriNMId == StationTriNMId);
        }

        public async Task<List<StationTriNM>> GetStationTriNMsByLocationAsync(string location)
        {
            return await _context.StationTriNMs
                .Where(s => s.Location.Contains(location) && s.IsActive == true)
                .ToListAsync();
        }

        public async Task<PagedResult<StationTriNM>> SearchStationsPagedAsync(
            string? name, 
            string? location, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Set<StationTriNM>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.StationTriNMName.Contains(name) || 
                                        s.StationTriNMCode.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(s => s.Address.Contains(location) || 
                                        s.City.Contains(location) || 
                                        s.Province.Contains(location));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            query = query.OrderBy(s => s.StationTriNMId);
            return await GetPagedResultAsync(query, pageNumber, pageSize);
        }
    }
}
