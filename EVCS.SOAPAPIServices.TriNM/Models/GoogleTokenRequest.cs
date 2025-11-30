using System.ComponentModel.DataAnnotations;

namespace EVCS.SOAPAPIServices.TriNM.Models
{
    public class GoogleTokenRequest
    {
        [Required]
        public string Credential { get; set; }
    }
}