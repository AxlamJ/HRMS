using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class SurveyQuestion
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("surveyId")]
        public int SurveyId { get; set; }

        [JsonProperty("questionText")]
        public string? QuestionText { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("questionType")]
        public string? QuestionType { get; set; } // Single Select, Multi Select, etc.

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }

        [JsonProperty("sortOrder")]
        public string? SortOrder { get; set; } // Ascending, Descending

        [JsonProperty("minValue")]
        public int? MinValue { get; set; } // For Slider

        [JsonProperty("maxValue")]
        public int? MaxValue { get; set; } // For Slider

        [JsonProperty("scale")]
        public int? Scale { get; set; } // For Rating (1-10)

        [JsonProperty("shape")]
        public string? Shape { get; set; } // Star, Smiley, Heart, Thumb

        [JsonProperty("label")]
        public string? Label { get; set; } // For Slider & Rating Labels

        [JsonProperty("weight")]
        public int? Weight { get; set; } // For Rating Weight

        [JsonProperty("createdById")]
        public int CreatedById { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

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
