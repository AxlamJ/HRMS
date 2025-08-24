using Microsoft.Data.SqlClient;
using System.Data;

namespace HrManagement.Data
{
    public class DataContext
    {
        private readonly IConfiguration _configuraiton;

        public DataContext(IConfiguration configuraiton)
        {
            _configuraiton = configuraiton;
        }
        public IDbConnection CreateConnection() => new SqlConnection(_configuraiton.GetConnectionString("HRMS"));
    }
}
