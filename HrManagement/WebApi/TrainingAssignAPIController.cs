using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingAssignAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        private ITrainingNotifactionRepository _trainingNotifactionRepository;
        public TrainingAssignAPIController(DataContext context, Email email, ITrainingNotifactionRepository trainingNotifactionRepository)
        {
            _context = context;
            _email = email;
            _trainingNotifactionRepository = trainingNotifactionRepository;
        }
        [HttpPost("AssignTraining")]
        public async Task<IActionResult> AssignTraining(TrainingAssignModel training)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (training != null)
                {
                    var UpdateQuery = @"UPDATE Trainings SET
                                           
                                                 VisibleTo = @VisibleTo
                                                ,Assigneby = @Assigneby
                                                ,Departments = @Departments
                                                ,DepartmentsSubCategories = @DepartmentsSubCategories
                                                ,Employees = @Employees
                                                ,Sites = @Sites
                                                ,AssigneDate = @AssigneDate
                                                 WHERE TrainingId = @AssigneId;";
                    training.Assigneby = loggedinUserId;
                    training.AssigneDate = DateTime.UtcNow;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, training);
                    connection.Close();


                    string sql = "SELECT Title FROM Trainings WHERE TrainingId = @TrainingId";

                    var parameters = new { TrainingId = training.AssigneId };
                    training.title = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);
                    var senEmail = await _trainingNotifactionRepository.SendEmail(training);
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                   
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("AssignCategory")]
        public async Task<IActionResult> AssignCategory(TrainingAssignModel training)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (training != null)
                {
                    var UpdateQuery = @"UPDATE TrainingStructureCategory SET
                                           
                                                 VisibleTo = @VisibleTo
                                                ,Assigneby = @Assigneby
                                                ,Departments = @Departments
                                                ,DepartmentsSubCategories = @DepartmentsSubCategories
                                                ,Employees = @Employees
                                                ,Sites = @Sites
                                                 WHERE Id = @AssigneId;";
                    training.Assigneby = loggedinUserId;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, training);
                    connection.Close();
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("AssignStructure")]
        public async Task<IActionResult> AssignStructure(TrainingAssignModel training)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (training != null)
                {
                    var UpdateQuery = @"UPDATE TrainingStructure SET
                                           
                                                 VisibleTo = @VisibleTo
                                                ,Assigneby = @Assigneby
                                                ,Departments = @Departments
                                                ,DepartmentsSubCategories = @DepartmentsSubCategories
                                                ,Employees = @Employees
                                                ,Sites = @Sites
                                                 WHERE TrainingStructureId = @AssigneId;";
                    training.Assigneby = loggedinUserId;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, training);
                    connection.Close();
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("GetTrainingAssignment")]
        public async Task<IActionResult> GetTrainingAssignment(int id)
        {
            try
            {
                var query = @"SELECT TrainingId AS AssigneId, VisibleTo, Assigneby, Departments, DepartmentsSubCategories, Employees, Sites
                          FROM Trainings WHERE TrainingId = @Id AND IsActive=1";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<TrainingAssignModel>(query, new { Id = id });

                if (result == null)
                    return NotFound(new { StatusCode = 404, Message = "Training assignment not found" });

                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Error fetching training assignment" });
            }
        }

        [HttpPost("GetCategoryAssignment")]
        public async Task<IActionResult> GetCategoryAssignment(int id)
        {
            try
            {
                var query = @"SELECT Id AS AssigneId, VisibleTo, Assigneby, Departments, DepartmentsSubCategories, Employees, Sites
                          FROM TrainingStructureCategory WHERE Id = @Id AND IsActive=1";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<TrainingAssignModel>(query, new { Id = id });

                if (result == null)
                    return NotFound(new { StatusCode = 404, Message = "Category assignment not found" });

                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Error fetching category assignment" });
            }
        }

        [HttpPost("GetStructureAssignment")]
        public async Task<IActionResult> GetStructureAssignment(int id)
        {
            try
            {
                var query = @"SELECT TrainingStructureId AS AssigneId, VisibleTo, Assigneby, Departments, DepartmentsSubCategories, Employees, Sites
                          FROM TrainingStructure WHERE TrainingStructureId = @Id AND IsActive=1";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<TrainingAssignModel>(query, new { Id = id });

                if (result == null)
                    return NotFound(new { StatusCode = 404, Message = "Structure assignment not found" });

                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Error fetching structure assignment" });
            }
        }

    }
}