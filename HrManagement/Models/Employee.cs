using Newtonsoft.Json;
using System.Globalization;

namespace HrManagement.Models
{
    public class Employee
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("profilePhotoUrl")]
        public string? ProfilePhotoUrl { get; set; }

        [JsonProperty("signaturePhotoUrl")]
        public string? SignaturePhotoUrl { get; set; }

        [JsonProperty("licensePhotoUrl")]
        public string? LicensePhotoUrl { get; set; }

        [JsonProperty("sinNo")]
        public string? SinNo { get; set; }

        [JsonProperty("sinDocumentName")]
        public string? SinDocumentName { get; set; }

        [JsonProperty("sinDocumentUrl")]
        public string? SinDocumentUrl { get; set; }

        [JsonProperty("chequeDocumentName")]
        public string? ChequeDocumentName { get; set; }

        [JsonProperty("chequeDocumentUrl")]
        public string? ChequeDocumentUrl { get; set; }

        [JsonProperty("federalTaxDocumentName")]
        public string? FederalTaxDocumentName { get; set; }

        [JsonProperty("federalTaxDocumentUrl")]
        public string? FederalTaxDocumentUrl { get; set; }

        [JsonProperty("albertaTaxDocumentName")]
        public string? AlbertaTaxDocumentName { get; set; }

        [JsonProperty("albertaTaxDocumentUrl")]
        public string? AlbertaTaxDocumentUrl { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("hiringDate")]
        public string? HiringDate { get; set; }

        [JsonProperty("dob")]
        public string? DOB { get; set; }

        [JsonProperty("onBoardingDate")]
        public string? OnBoardingDate { get; set; }

        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("countryId")]
        public string? CountryId { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("postalCode")]
        public string? PostalCode { get; set; }

        [JsonProperty("officePhoneNumber")]
        public string? OfficePhoneNumber { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("extension")]
        public string? Extension { get; set; }

        [JsonProperty("departmentId")]
        public int? DepartmentId { get; set; }

        [JsonProperty("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonProperty("departmentSubCategoryId")]
        public int? DepartmentSubCategoryId { get; set; }

        [JsonProperty("departmentSubCategoryName")]
        public string? DepartmentSubCategoryName { get; set; }

        [JsonProperty("siteId")]
        public int? SiteId { get; set; }

        [JsonProperty("siteName")]
        public string? SiteName { get; set; }

        [JsonProperty("address")]
        public string? Address { get; set; }

        [JsonProperty("positionId")]
        public int? PositionId { get; set; }

        [JsonProperty("positionName")]
        public string? PositionName { get; set; }

        [JsonProperty("maritalStatus")]
        public string? MaritalStatus { get; set; }

        [JsonProperty("managerId")]
        public int? ManagerId { get; set; }

        [JsonProperty("managerName")]
        public string? ManagerName { get; set; }

        [JsonProperty("alternativeEmail")]
        public string? AlternativeEmail { get; set; }

        [JsonProperty("probationDateStart")]
        public string? ProbationDateStart { get; set; }

        [JsonProperty("probationDateEnd")]
        public string? ProbationDateEnd { get; set; }

        [JsonProperty("acceptanceDate")]
        public string? AcceptanceDate { get; set; }

        [JsonProperty("employmentStatusId")]
        public int? EmploymentStatusId { get; set; }

        [JsonProperty("employmentStatus")]
        public string? EmploymentStatus { get; set; }

        [JsonProperty("employmentLevelId")]
        public int? EmploymentLevelId { get; set; }

        [JsonProperty("employmentLevel")]
        public string? EmploymentLevel { get; set; }

        [JsonProperty("timezone")]
        public string? TimeZone { get; set; }

        [JsonProperty("timezoneOffSet")]
        public string? TimeZoneOffset { get; set; }

        [JsonProperty("timeZoneName")]
        public string? TimeZoneName { get; set; }

        [JsonProperty("sponsorShip")]
        public string? SponsorShip { get; set; }

        [JsonProperty("workEligibility")]
        public string? WorkEligibility { get; set; }

        [JsonProperty("immigrationStatus")]
        public string? ImmigrationStatus { get; set; }

        [JsonProperty("other")]
        public string? Other { get; set; }

        [JsonProperty("registrationDate")]
        public string? RegistrationDate { get; set; }

        [JsonProperty("registrationNumber")]
        public string? RegistrationNumber { get; set; }

        [JsonProperty("liabilityInsuranceName")]
        public string? LiabilityInsuranceName { get; set; }

        [JsonProperty("liabilityInsuranceUrl")]
        public string? LiabilityInsuranceUrl { get; set; }

        [JsonProperty("registrationNumberName")]
        public string? RegistrationNumberName { get; set; }

        [JsonProperty("registrationNumberUrl")]
        public string? RegistrationNumberUrl { get; set; }

        [JsonProperty("businessName")]
        public string? BusinessName { get; set; }

        [JsonProperty("businessEmail")]
        public string? BusinessEmail { get; set; }

        [JsonProperty("businessNumber")]
        public string? BusinessNumber { get; set; }

        [JsonProperty("businessChequeName")]
        public string? BusinessChequeName { get; set; }

        [JsonProperty("businessChequeUrl")]
        public string? BusinessChequeUrl { get; set; }

        [JsonProperty("contractorBusinessName")]
        public string? ContractorBusinessName { get; set; }

        [JsonProperty("contractorBusinessEmail")]
        public string? ContractorBusinessEmail { get; set; }

        [JsonProperty("contractorBusinessNumber")]
        public string? ContractorBusinessNumber { get; set; }

        [JsonProperty("contractorBusinessChequeName")]
        public string? ContractorBusinessChequeName { get; set; }

        [JsonProperty("contractorBusinessChequeUrl")]
        public string? ContractorBusinessChequeUrl { get; set; }

        [JsonProperty("terminationDismissalDate")]
        public DateTime? TerminationDismissalDate { get; set; }

        [JsonProperty("terminationDismissalReasonId")]
        public int? TerminationDismissalReasonId { get; set; }

        [JsonProperty("terminationDismissalReason")]
        public string? TerminationDismissalReason { get; set; }

        [JsonProperty("terminationDismissalType")]
        public string? TerminationDismissalType { get; set; }

        [JsonProperty("terminationDismissalComment")]
        public string? TerminationDismissalComment { get; set; }

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

        [JsonProperty("status")]
        public int Status { get; set; }

        //For User Creation

        [JsonProperty("createUser")]
        public bool CreateUser { get; set; }

        [JsonProperty("roleId")]
        public int? RoleId { get; set; }

        [JsonProperty("userRoles")]
        public string? UserRoles { get; set; }

        [JsonProperty("roleName")]
        public string? RoleName { get; set; }

        [JsonProperty("salt")]
        public string? Salt { get; set; }

        [JsonProperty("passwordHash")]
        public string? PasswordHash { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

    }

    public class EmployeeFilter : FilterBase
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("departmentId")]
        public int? DepartmentId { get; set; }

        [JsonProperty("siteId")]
        public List<int>? SiteId { get; set; }

        [JsonProperty("positionId")]
        public int? PositionId { get; set; }

        [JsonProperty("employmentStatusId")]
        public int? EmploymentStatusId { get; set; }

        [JsonProperty("formerEmployee")]
        public int? FormerEmployee { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

    }
}
