using EVCS.SOAP.WebMVC.TriNM.SOAPModels;
using EVCS.SOAPAPIServices.TriNM.Services;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace EVCS.SOAP.WebMVC.TriNM.SOAPServices;

public class StationSoapClient : IStationSoapClient
{
    private readonly string _soapServiceUrl;
    private readonly BasicHttpBinding _binding;
    private readonly EndpointAddress _endpointAddress;

    public StationSoapClient(IConfiguration configuration)
    {
        _soapServiceUrl = configuration["SOAPService:BaseUrl"] 
            ?? throw new InvalidOperationException("SOAPService:BaseUrl is not configured");

        _binding = new BasicHttpBinding
        {
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue,
            ReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxDepth = int.MaxValue,
                MaxStringContentLength = int.MaxValue,
                MaxArrayLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue,
                MaxNameTableCharCount = int.MaxValue
            },
            Security = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.None
            }
        };

        _endpointAddress = new EndpointAddress($"{_soapServiceUrl}/StationService.asmx");
    }

    private IStationSOAPServices CreateChannel()
    {
        var channelFactory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
        channelFactory.Open();
        return channelFactory.CreateChannel();
    }

    private static void CloseChannel(ICommunicationObject? channel)
    {
        if (channel == null) return;
        
        try
        {
            if (channel.State == CommunicationState.Opened)
            {
                channel.Close();
            }
            else if (channel.State == CommunicationState.Faulted)
            {
                channel.Abort();
            }
        }
        catch
        {
            channel.Abort();
        }
    }

    private static void CloseFactory(ChannelFactory<IStationSOAPServices>? factory)
    {
        if (factory == null) return;
        
        try
        {
            if (factory.State == CommunicationState.Opened)
            {
                factory.Close();
            }
            else if (factory.State == CommunicationState.Faulted)
            {
                factory.Abort();
            }
        }
        catch
        {
            factory.Abort();
        }
    }

    public async Task<List<Station>> GetAllAsync()
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var result = await channel.GetAllAsync();
            return MapToLocalStations(result);
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to get all stations: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<Station?> GetByIdAsync(int id)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var result = await channel.GetAsync(id);
            return result != null ? MapToLocalStation(result) : null;
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to get station by id {id}: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<PagedStationResult> SearchStationsPagedAsync(string? name, string? location, bool? isActive, int pageNumber, int pageSize)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var result = await channel.SearchStationsPagedAsync(name, location, isActive, pageNumber, pageSize);
            return new PagedStationResult
            {
                Items = result.Items.Select(MapToLocalStation).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to search stations: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<PagedStationResult> GetStationsPagedAsync(int pageNumber, int pageSize)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var result = await channel.GetStationsPagedAsync(pageNumber, pageSize);
            return new PagedStationResult
            {
                Items = result.Items.Select(MapToLocalStation).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to get stations: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<int> CreateAsync(Station station)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var soapStation = MapToSoapStation(station);
            var result = await channel.CreateStationAsync(soapStation);
            return result;
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to create station: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<Station> UpdateAsync(int id, Station station)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            station.StationId = id;
            var soapStation = MapToSoapStation(station);
            await channel.UpdateStationAsync(soapStation);
            
            // Get updated station
            var updated = await channel.GetAsync(id);
            return updated != null ? MapToLocalStation(updated) : station;
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to update station: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        IStationSOAPServices? channel = null;
        ChannelFactory<IStationSOAPServices>? factory = null;
        try
        {
            factory = new ChannelFactory<IStationSOAPServices>(_binding, _endpointAddress);
            factory.Open();
            channel = factory.CreateChannel();
            var result = await channel.DeleteStationAsync(id);
            return result > 0;
        }
        catch (FaultException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FaultException($"Failed to delete station: {ex.Message}");
        }
        finally
        {
            CloseChannel(channel as ICommunicationObject);
            CloseFactory(factory);
        }
    }

    #region Mapping Methods

    private static Station MapToLocalStation(EVCS.SOAPAPIServices.TriNM.SOAPModels.Station soapStation)
    {
        return new Station
        {
            StationId = soapStation.StationId,
            StationCode = soapStation.StationCode ?? string.Empty,
            StationName = soapStation.StationName ?? string.Empty,
            Address = soapStation.Address ?? string.Empty,
            City = soapStation.City,
            Province = soapStation.Province,
            Latitude = soapStation.Latitude,
            Longitude = soapStation.Longitude,
            Capacity = soapStation.Capacity,
            CurrentAvailable = soapStation.CurrentAvailable,
            Owner = soapStation.Owner ?? string.Empty,
            ContactPhone = soapStation.ContactPhone,
            ContactEmail = soapStation.ContactEmail,
            Description = soapStation.Description,
            CreatedDate = soapStation.CreatedDate,
            ModifiedDate = soapStation.ModifiedDate,
            IsActive = soapStation.IsActive
        };
    }

    private static List<Station> MapToLocalStations(List<EVCS.SOAPAPIServices.TriNM.SOAPModels.Station> soapStations)
    {
        return soapStations.Select(MapToLocalStation).ToList();
    }

    private static EVCS.SOAPAPIServices.TriNM.SOAPModels.Station MapToSoapStation(Station localStation)
    {
        return new EVCS.SOAPAPIServices.TriNM.SOAPModels.Station
        {
            StationId = localStation.StationId,
            StationCode = localStation.StationCode,
            StationName = localStation.StationName,
            Address = localStation.Address,
            City = localStation.City,
            Province = localStation.Province,
            Latitude = localStation.Latitude,
            Longitude = localStation.Longitude,
            Capacity = localStation.Capacity,
            CurrentAvailable = localStation.CurrentAvailable,
            Owner = localStation.Owner,
            ContactPhone = localStation.ContactPhone,
            ContactEmail = localStation.ContactEmail,
            Description = localStation.Description,
            CreatedDate = localStation.CreatedDate,
            ModifiedDate = localStation.ModifiedDate,
            IsActive = localStation.IsActive
        };
    }

    #endregion
}

