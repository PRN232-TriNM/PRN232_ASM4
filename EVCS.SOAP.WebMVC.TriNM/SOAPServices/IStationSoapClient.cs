using EVCS.SOAP.WebMVC.TriNM.SOAPModels;

namespace EVCS.SOAP.WebMVC.TriNM.SOAPServices;

public interface IStationSoapClient
{
    Task<List<Station>> GetAllAsync();
    Task<Station?> GetByIdAsync(int id);
    Task<PagedStationResult> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize);
    Task<PagedStationResult> GetStationsPagedAsync(int pageNumber, int pageSize);
    Task<int> CreateAsync(Station station);
    Task<Station> UpdateAsync(int id, Station station);
    Task<bool> DeleteAsync(int id);
}

