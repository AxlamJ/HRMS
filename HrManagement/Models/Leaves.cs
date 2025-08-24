using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class Leaves
    {
        [JsonProperty("leaveId")]
        public int? LeaveId { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("employeeName")]
        public string? EmployeeName { get; set; }

        [JsonProperty("leaveTypeId")]
        public int? LeaveTypeId { get; set; }

        [JsonProperty("leaveTypeName")]
        public string? LeaveTypeName { get; set; }

        [JsonProperty("leaveTypeShortName")]
        public string? LeaveTypeShortName { get; set; }

        [JsonProperty("startDate")]
        public string? StartDate { get; set; }

        [JsonProperty("endDate")]
        public string? EndDate { get; set; }

        [JsonProperty("totalDays")]
        public int? TotalDays { get; set; }

        [JsonProperty("leaveStatusId")]
        public int? LeaveStatusId { get; set; }

        [JsonProperty("leaveStatusName")]
        public string? LeaveStatusName { get; set; }

        [JsonProperty("leaveReason")]
        public string? LeaveReason { get; set; }

        [JsonProperty("approvedById")]
        public int? ApprovedById { get; set; }

        [JsonProperty("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonProperty("approvedDate")]
        public DateTime? ApprovedDate { get; set; }

        [JsonProperty("comments")]
        public string? Comments { get; set; }

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

    public class ApproveRejectLeaves
    {
        [JsonProperty("leaveId")]
        public int? LeaveId { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("leaveStatusId")]
        public int? LeaveStatusId { get; set; }

        [JsonProperty("leaveStatusName")]
        public string? LeaveStatusName { get; set; }

        [JsonProperty("approvedById")]
        public int? ApprovedById { get; set; }

        [JsonProperty("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonProperty("comments")]
        public string? Comments { get; set; }

        [JsonProperty("approvedDate")]
        public DateTime? ApprovedDate { get; set; }
    }

    public class LeavesFilter : FilterBase
    {

        [JsonProperty("employeeCode")]
        public List<int>? EmployeeCode { get; set; }

        [JsonProperty("startDate")]
        public string? StartDate { get; set; }

        [JsonProperty("endDate")]
        public string? EndDate { get; set; }

        [JsonProperty("leaveTypeId")]
        public int? LeaveTypeId { get; set; }

        [JsonProperty("leaveStatusId")]
        public int? LeaveStatusId { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public class LeaveTypes
    {
        [JsonProperty("leaveTypeId")]
        public int? LeaveTypeId { get; set; }

        [JsonProperty("leaveTypeName")]
        public string? LeaveTypeName { get; set; }

        [JsonProperty("leaveTypeShortName")]
        public string? LeaveTypeShortName { get; set; }

        [JsonProperty("autoApprove")]
        public bool? AutoApprove { get; set; }

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
    public class LeaveTypesFilter :FilterBase
    {

        [JsonProperty("leaveTypeName")]
        public string? LeaveTypeName { get; set; }

        [JsonProperty("autoApprove")]
        public bool? AutoApprove { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

    }

    public class LeavesPolicy
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("policyName")]
        public string? PolicyName { get; set; }

        [JsonProperty("policyTypeId")]
        public int? PolicyTypeId { get; set; }

        [JsonProperty("policyType")]
        public string? PolicyType { get; set; }

        [JsonProperty("policyDays")]
        public int? PolicyDays { get; set; }

        [JsonProperty("policyPeriod")]
        public string? PolicyPeriod { get; set; }

        [JsonProperty("applyDays")]
        public string? ApplyDays { get; set; }

        [JsonProperty("movetoNextPeriod")]
        public bool? MovetoNextPeriod { get; set; }

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
    public class LeavesPolicyFilter: FilterBase
    {

        [JsonProperty("policyName")]
        public string? PolicyName { get; set; }

        [JsonProperty("policyTypeId")]
        public int? PolicyTypeId { get; set; }

        [JsonProperty("policyType")]
        public string? PolicyType { get; set; }

        [JsonProperty("policyPeriod")]
        public string? PolicyPeriod { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }
    }

}
