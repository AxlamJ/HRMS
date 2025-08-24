using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace HrManagement.Models
{
    public class Sites
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("siteName")]
        public string? SiteName { get; set; }

        [JsonProperty("countryId")]
        public string? CountryId { get; set; }

        [JsonProperty("countryName")]
        public string? CountryName { get; set; }

        [JsonProperty("timeZoneId")]
        public string? TimeZoneId { get; set; }

        [JsonProperty("timeZoneName")]
        public string? TimeZoneName { get; set; }

        [JsonProperty("timeZoneOffset")]
        public string? TimeZoneOffset { get; set; }

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

    public class SitesFilter : FilterBase
    {
        [JsonProperty("siteName")]
        public string? SiteName { get; set; }

        [JsonProperty("countryId")]
        public string? CountryId { get; set; }

        [JsonProperty("timeZoneId")]
        public string? TimeZoneId { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }
    }
}
