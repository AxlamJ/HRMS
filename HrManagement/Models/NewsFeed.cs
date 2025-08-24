using System;
using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class NewsFeed
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("newsTitle")]
        public string? NewsTitle { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonProperty("youtubeUrl")]
        public string? YoutubeUrl { get; set; }

        [JsonProperty("videoFileUrl")]
        public string? VideoFileUrl { get; set; }

        [JsonProperty("pinToTop")]
        public bool? PinToTop { get; set; }

        [JsonProperty("visibleTo")]
        public string? VisibleTo { get; set; }

        [JsonProperty("departments")]
        public string? Departments { get; set; }

        [JsonProperty("departmentsSubCategories")]
        public string? DepartmentsSubCategories { get; set; }

        [JsonProperty("employees")]
        public string? Employees { get; set; }

        [JsonProperty("sites")]
        public string? Sites { get; set; }

        [JsonProperty("createdById")]
        public int? CreatedById { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("createdByDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("modifiedByDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; } = true; // Default to true
    }
}
