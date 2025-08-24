using System;
using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class SupportingDocument
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("documentName")]
        public string? DocumentName { get; set; }

        [JsonProperty("documentFileName")]
        public string? DocumentFileName { get; set; }

        [JsonProperty("documentUrl")]
        public string? DocumentUrl { get; set; }

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

        [JsonProperty("isDeletedApproved")]
        public bool? IsDeletedApproved { get; set; }
    }

}
