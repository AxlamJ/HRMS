using Dapper;
using HrManagement.Controllers;
using HrManagement.Data;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Helpers
{
    public class Common
    {
        private readonly DataContext _context;
        public Common(DataContext context)
        {
            _context = context;
        }

        // Generic method to fetch data from the database
        public async Task<List<T>> GetAllAsync<T>(string queryname, HttpContext context)
        {
            var query = await CreateQueries(queryname, context);
            if (!string.IsNullOrEmpty(query))
            {
                using var connection = _context.CreateConnection();
                connection.Open();

                var data = await connection.QueryAsync<T>(query);
                connection.Close();

                return data.AsList();

            }
            else
            {
                return new List<T>();
            }
        }

        private async Task<string> CreateQueries(string queryname, HttpContext httpContext)
        {

            if (queryname == "UserLessonProgressReport")
            {
                return @"   Select 
                             u.LessonID AS CourseId,
                             u.UserID AS EmployeeId,
                             ISNULL(u.Watched, 0) AS IsCompleted
                             from UserLessonProgress u
                             Inner Join TrainingStructureCategory c on c.Id = u.LessonID 
                             Inner Join TrainingStructure s on s.TrainingStructureId = c.TrainingStructureId   
                             Inner Join Trainings t on t.TrainingId = s.TrainingId  
                             Where u.IsActive = 1 AND t.IsActive=1 AND s.Status=1";
            }
            if (queryname == "TrainingStructure")
            {
                return @"   Select 
         
                          t.TrainingId AS TrainingId , 
                          c.Title AS CourseTitle,
                          c.Id AS CourseId,
                          c.Type CourseType
                          from TrainingStructure t
                          Inner Join TrainingStructureCategory c on t.TrainingStructureId = c.TrainingStructureId                         
                          Inner Join Trainings tr on tr.TrainingId = t.TrainingId                         
                          Where t.Status='1' AND c.Status='1' AND t.IsActive=1 AND c.IsActive=1 AND tr.IsActive=1";
            }
            if (queryname == "UserQuizAttempt")
            {
                return @"SELECT 
                                 a.Id AS QuizID,
                                 q.UserID AS EmployeeId,
                                 q.Score AS SecureScore,
                                 q.PassScore AS PassScore,
                                 100 AS TotalScore,
                                 c.Id AS CourseId,
                                 a.Title AS QuizTitle
                                 FROM TrainingStructureCategory c
                                 LEFT JOIN Assessment a ON c.Id = a.CategoryId AND c.Type = '3' AND a.IsActive=1
                                 LEFT JOIN UserQuizAttempt q ON a.Id = q.QuizID AND q.IsActive=1
                                 Inner Join TrainingStructure s on s.TrainingStructureId = c.TrainingStructureId   
                                 Inner Join Trainings t on t.TrainingId = s.TrainingId   
                                 WHERE c.IsActive=1 AND t.IsActive=1
                             ";
            }
            if (queryname == "TrainingsReports")
            {
                return @"Select 
                         t.TrainingId AS TrainingId,
                         t.Title AS TrainingTitle,
                         t.Departments,
                         t.Employees,
                         t.Sites
                         from Trainings t Where IsActive = 1 AND IsApproved=0";
            }
            if (queryname == "Trainings")
            {
                return "Select * from Trainings Where IsActive = 1 AND IsApproved=0";
            }
            if (queryname == "GetTrainingWithPermission")
            {
                return @"Select t.TrainingId, t.Title, t.ItemType, p.PermissionId, p.IsAssigned, p.AllowedRole from Trainings t
                                LEFT JOIN Permissions p ON p.ItemId = t.TrainingId AND p.ItemType='1' AND p.IsActive=1
                         Where t.IsActive = 1 AND t.IsApproved=0";
            }
            if (queryname == "Departments")
            {
                return "Select * from Department Where IsActive = 1 ";
            }
            if (queryname == "DepartmentSubCategories")
            {
                return "Select * from DepartmentSubCategories Where IsActive = 1 ";
            }
            else if (queryname == "Sites")
            {
                return "Select * from Sites Where IsActive = 1 ";
            }
            else if (queryname == "UserSites")
            {
                var UserSites = httpContext.Session.GetString("UserSites");

                return "Select * from Sites Where IsActive = 1 ";
            }
            else if (queryname == "Employees")
            {
                return "Select * from Employee Where Status = 1";
            }
            else if (queryname == "UserSiteEmployees")
            {
                var UserSites = httpContext.Session.GetString("UserSites");

                if (!string.IsNullOrEmpty(UserSites))
                {
                    var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                    return @"SELECT *
                        FROM Employee
                        CROSS APPLY OPENJSON(SiteName)
                        WITH (
                            id INT,
                            name NVARCHAR(200)
                        ) AS parsed
                        WHERE parsed.id IN (" + string.Join(",", EmployeeSites.Select(s => s.id)) + ") and Status = 1";

                }
                else
                {
                    return "";
                }
            }
            else if (queryname == "Managers")
            {
                //return "Select * from Employee Where Status = 1 and PositionId = 16";
                return "Select * from Employee Where Status = 1 and Lower(PositionName) like '%manager%'";
            }
            else if (queryname == "UserSiteManagers")
            {
                var UserSites = httpContext.Session.GetString("UserSites");

                if (!string.IsNullOrEmpty(UserSites))
                {
                    var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);
                    return @"SELECT *
                        FROM Employee
                        CROSS APPLY OPENJSON(SiteName)
                        WITH (
                            id INT,
                            name NVARCHAR(200)
                        ) AS parsed
                        WHERE parsed.id IN (" + string.Join(",", EmployeeSites.Select(s => s.id)) + ") and Status = 1 and Lower(PositionName) like '%manager%'";

                }
                else
                {
                    return "";
                }
            }
            else if (queryname == "EmployeeLevels")
            {
                return "Select * from EmployeeLevel";
            }
            else if (queryname == "EmployeePosition")
            {
                return "Select * from EmployeePosition Where IsActive = 1 ";
            }
            else if (queryname == "EmployeeStatus")
            {
                return "Select * from EmployeeStatus";
            }
            else if (queryname == "Roles")
            {
                return "Select * from Roles Where IsActive = 1 ";
            }
            else if (queryname == "TerminationDismissalReason")
            {
                return "Select * from TerminationDismissalReason Where IsActive = 1";
            }
            else
            {
                return "";
            }
        }

        public async Task<int> GetEmployeeCodeByUserId<T>(int loggedinUserId)
        {
            var employeeCodeQuery = "SELECT EmployeeCode FROM Users WHERE UserId = @UserId";
            int employeeCode = 0;

            using var connection = _context.CreateConnection();
            connection.Open();
            employeeCode = await connection.QuerySingleAsync<int>(employeeCodeQuery, new { UserId = loggedinUserId });
            connection.Close();

            return employeeCode;
        }
    }
}
