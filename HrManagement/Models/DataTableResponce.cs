using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class DataTableResponce
    {
        //public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public string Message { get; set; }
        public object aaData { get; set; }
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
