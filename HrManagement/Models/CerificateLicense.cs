using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class CerificateLicense
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("issuingOrganization")]
        public string? IssuingOrganization { get; set; }

        [JsonProperty("credentialId")]
        public string? CredentialId { get; set; }

        [JsonProperty("uRL")]
        public string? URL { get; set; }

        [JsonProperty("issueDate")]
        public string? IssueDate { get; set; }

        [JsonProperty("expirationDate")]
        public string? ExpirationDate { get; set; }

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
