using System.Text.Json.Serialization;

namespace HrManagement.Models
{
    public class EmployeeEquipment
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonPropertyName("equipmentName")]
        public string? EquipmentName { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("statusUpdatedById")]
        public int? StatusUpdatedById { get; set; }

        [JsonPropertyName("statusUpdatedBy")]
        public string? StatusUpdatedBy { get; set; }

        [JsonPropertyName("statusUpdatedDate")]
        public DateTime? StatusUpdatedDate { get; set; }

        [JsonPropertyName("createdById")]
        public int? CreatedById { get; set; }

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }
    }
}
