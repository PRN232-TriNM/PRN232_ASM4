using System.Runtime.Serialization;

namespace EVCS.SOAP.WebMVC.TriNM.SOAPModels
{
    [DataContract]
    public class PagedStationResult
    {
        [DataMember]
        public List<Station> Items { get; set; } = new();

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public int TotalPages { get; set; }

        [DataMember]
        public bool HasPreviousPage { get; set; }

        [DataMember]
        public bool HasNextPage { get; set; }
    }
}

