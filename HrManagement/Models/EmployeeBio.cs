using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EmployeeBio
    {

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("about")]
        public string? About { get; set; }

        [JsonProperty("hobbies")]
        public string? Hobbies { get; set; }

        [JsonProperty("favoriteBooks")]
        public string? FavoriteBooks { get; set; }

        [JsonProperty("musicPreference")]
        public string? MusicPreference { get; set; }

        [JsonProperty("Sports")]
        public string? Sports { get; set; }

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

    }
}
