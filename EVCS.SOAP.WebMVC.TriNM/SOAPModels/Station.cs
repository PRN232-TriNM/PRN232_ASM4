using System.Runtime.Serialization;

namespace EVCS.SOAP.WebMVC.TriNM.SOAPModels;

[DataContract]
public class Station
{
    [DataMember]
    public int StationId { get; set; }

    [DataMember]
    public string StationCode { get; set; } = string.Empty;

    [DataMember]
    public string StationName { get; set; } = string.Empty;

    [DataMember]
    public string Address { get; set; } = string.Empty;

    [DataMember]
    public string? City { get; set; }

    [DataMember]
    public string? Province { get; set; }

    [DataMember]
    public decimal? Latitude { get; set; }

    [DataMember]
    public decimal? Longitude { get; set; }

    [DataMember]
    public int Capacity { get; set; }

    [DataMember]
    public int CurrentAvailable { get; set; }

    [DataMember]
    public string Owner { get; set; } = string.Empty;

    [DataMember]
    public string? ContactPhone { get; set; }

    [DataMember]
    public string? ContactEmail { get; set; }

    [DataMember]
    public string? Description { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime? ModifiedDate { get; set; }

    [DataMember]
    public bool IsActive { get; set; }
}

