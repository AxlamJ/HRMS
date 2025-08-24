using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class FilterBase
    {
        public int? iDisplayStart { get; set; }
        public int? iDisplayLength { get; set; }
        public string? SortCol { get; set; }
        public string? sSortDir_0 { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }

        [JsonProperty("offset")]
        public int? Offset { get; set; }
    }
}
