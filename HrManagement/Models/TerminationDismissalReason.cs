using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class TerminationDismissalReason
    {
        [JsonProperty("id")]
        public int? Id { get; set; } 
        
        [JsonProperty("id")]
        public string? ReasonName { get; set; } 
        
        [JsonProperty("id")]
        public bool IsActive { get; set; }   
        
        [JsonProperty("createdById")]
        public int? CreatedById { get; set; }   
        
        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }   
        
        [JsonProperty("createdDate")]
        public string? CreatedDate { get; set; } 
        
        [JsonProperty("modifiedById")]
        public int? ModifiedById { get; set; }   
        
        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }   
        
        [JsonProperty("modifiedDate")]
        public string? ModifiedDate { get; set; }
    }
}
