using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EmployeePosition
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("positionName")]
        public string? PositionName { get; set; }

        [JsonProperty("createdById")]
        public int? CreatedById { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("isActiveApproved")]
        public bool? IsActiveApproved { get; set; }
    }

    public class EmployeePositionFilter : FilterBase
    {

        [JsonProperty("positionName")]
        public string? PositionName { get; set; }
        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

    }
}
