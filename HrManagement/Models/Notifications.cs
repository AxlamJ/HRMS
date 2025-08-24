using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class Notifications
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("notificationType")]
        public string? NotificationType { get; set; }

        [JsonProperty("notificationDescription")]
        public string? NotificationDescription { get; set; }

        [JsonProperty("notificationUrl")]
        public string? NotificationUrl { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }
    }
}
