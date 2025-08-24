using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class StatsVm
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("value")]
        public double value { get; set; }

        [JsonProperty("totalcount")]
        public double totalcount { get; set; }
    }

    public class MonthlyHiredLeft
    {
        public string MonthYear { get; set; }  // e.g., "Apr 2024"
        public int Hired { get; set; }
        public int Left { get; set; }
    }
    public class UpCommingoccasion
    {
        public string EmployeeName { get; set; }  // e.g., "Apr 2024"
        public string EventDate { get; set; }
        public string EventType { get; set; }
    }  
    public class EmployeeCount
    {
        public string TotalEmployees { get; set; }  // e.g., "Apr 2024"
        public string NewEmployeesCount { get; set; }
    }
    public class LineCharts
    {
        public LineCharts()
        {
            xAxis = new List<string>();
            yAxisSeries = new List<double>();
        }
        public List<string> xAxis { get; set; }
        public List<double> yAxisSeries { get; set; }
    }

    public class DashboardStats
    {
        public DashboardStats()
        {
            GenderStats = new List<StatsVm>();
            AgeStats = new List<StatsVm>();
            SiteStats = new List<StatsVm>();
            DepartmentStats = new List<StatsVm>();
            HeadCountStats = new LineCharts();
            HiredVsLeft = new List<MonthlyHiredLeft>();
            UpComingOccasions = new List<UpCommingoccasion>();
            EmployeesCount = new EmployeeCount();
        }
        public List<StatsVm> GenderStats { get; set; }
        public List<StatsVm> AgeStats { get; set; }
        public List<StatsVm> SiteStats { get; set; }
        public List<StatsVm> DepartmentStats { get; set; }
        public LineCharts HeadCountStats { get; set; }
        public List<MonthlyHiredLeft> HiredVsLeft { get; set; }
        public List<UpCommingoccasion> UpComingOccasions { get; set; }
        public EmployeeCount EmployeesCount { get; set; }
    }
}
