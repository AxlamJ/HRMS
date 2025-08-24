using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class SurveyResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("surveyId")]
        public int SurveyId { get; set; }  // Foreign key reference to Surveys

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("questionId")]
        public int QuestionId { get; set; }  // Foreign key reference to SurveyQuestions

        [JsonProperty("answerText")]
        public string? AnswerText { get; set; }

        [JsonProperty("optionId")]
        public int? OptionId { get; set; }

        [JsonProperty("responseDate")]
        public DateTime ResponseDate { get; set; }

        [JsonProperty("createdById")]
        public int CreatedById { get; set; }

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
}
