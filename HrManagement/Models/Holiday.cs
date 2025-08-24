using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrManagement.Models
{
    public class Holiday
    {
        [JsonProperty("holidayId")]
        public int HolidayId { get; set; }

        [JsonProperty("holidayName")]
        public string? HolidayName { get; set; }

        [JsonProperty("holidayDate")]
        public DateTime? HolidayDate { get; set; }

        [JsonProperty("isRecurring")]
        public bool? IsRecurring { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

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
        public bool IsActive { get; set; }
    }

    public class HolidayFilter : FilterBase
    {
        public string? HolidayName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
