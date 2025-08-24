using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Controllers;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        public DepartmentAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("UpsertDepartment")]
        public async Task<IActionResult> UpsertDepartment(Department department)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (department != null)
                {
                    if (department.DepartmentId == null)
                    {
                        var InsertQuery = @"INSERT INTO Department (
                                            DepartmentName
                                           ,CreatedById
                                           ,CreatedBy
                                           ,CreatedDate
                                           ,ModifiedById
                                           ,ModifiedBy
                                           ,ModifiedDate
                                           ,IsActive
                                           ,IsActiveApproved) 
                                           VALUES (
                                            @DepartmentName
                                           ,@CreatedById
                                           ,@CreatedBy
                                           ,@CreatedDate
                                           ,@ModifiedById
                                           ,@ModifiedBy
                                           ,@ModifiedDate
                                           ,@IsActive
                                           ,@IsActiveApproved)";

                        department.CreatedById = loggedinUserId;
                        department.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        department.CreatedDate = DateTime.UtcNow;
                        department.ModifiedById = loggedinUserId;
                        department.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        department.ModifiedDate = DateTime.UtcNow;
                        department.IsActive = true;
                        department.IsActiveApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, department);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE Department SET
                                                DepartmentName = @DepartmentName  
                                               ,ModifiedById = @ModifiedById 
                                               ,ModifiedBy = @ModifiedBy 
                                               ,ModifiedDate = @ModifiedDate 
                                               ,IsActive = @IsActive 
                                                WHERE DepartmentId = @DepartmentId";
                        department.ModifiedById = loggedinUserId;
                        department.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        department.ModifiedDate = DateTime.UtcNow;
                        department.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, department);
                        connection.Close();
                    }


                }

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    //Message = "Site created successfully!",
                    //Data = new { Id = productId }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);

                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpGet("GetDepartmentById")]
        public async Task<ActionResult> GetDepartmentById(string DepartmentId)
        {
            try
            {
                var departmentquery = "Select * from Department Where DepartmentId = @DepartmentId";
                var paramas = new { DepartmentId = DepartmentId };
                using var connection = _context.CreateConnection();
                connection.Open();
                var departmentdata = await connection.QueryFirstOrDefaultAsync<Department>(departmentquery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Department = departmentdata
                });
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

        [HttpPost("DeleteDepartmentById")]
        public async Task<ActionResult> DeleteDepartmentById(string DepartmentId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var departmentquery = @"Update Department SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where DepartmentId = @DepartmentId";
                var paramas = new { DepartmentId = DepartmentId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);

                var subCategoryquery = @"Update DepartmentSubCategories SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where DepartmentId = @DepartmentId";
                await connection.ExecuteAsync(subCategoryquery, paramas);
                connection.Close();


                var query = "Select * from Department where DepartmentId = @DepartmentId";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var deptData = await connection.QueryFirstOrDefaultAsync<Department>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(deptData);

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
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

        [HttpPost("ApproveDeleteDepartmentById")]
        public async Task<ActionResult> ApproveDeleteDepartmentById(string DepartmentId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                var query = @"Select E.Email,E.FirstName, E.LastName from Department D 
                            JOIN USERS U ON U.UserId = D.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where D.DepartmentId = @DepartmentId";

                var _params = new { DepartmentId = DepartmentId };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();


                var departmentquery = @"Update Department SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where DepartmentId = @DepartmentId";
                var paramas = new { DepartmentId = DepartmentId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);

                var subCategoryquery = @"Update DepartmentSubCategories SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where DepartmentId = @DepartmentId";
                await connection.ExecuteAsync(subCategoryquery, paramas);
                connection.Close();


                var _query = "Select * from Department where DepartmentId = @DepartmentId";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var deptdata = await connection.QueryFirstOrDefaultAsync<Department>(_query, paramas);
                __connection.Close();

                await SendDepartmentDeletionApprovalEmail(employee, deptdata);

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
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

        [HttpPost("GetDepartments")]
        public async Task<IActionResult> GetDepartments(DepartmentsFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterDepartmentsData(filters);
                dtr.iTotalRecords = tuple.Item2;
                dtr.iTotalDisplayRecords = tuple.Item2;
                dtr.aaData = tuple.Item1;
                return Ok(dtr);

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
        private async Task<Tuple<List<Department>, int>> FilterDepartmentsData(DepartmentsFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where D.IsActiveApproved = 0 ";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.DepartmentName))
                {
                    whereClause += "\n AND D.DepartmentName like '%'+@DepartmentName+'%'";
                    sqlParams.Add(new SqlParameter("@DepartmentName", search.DepartmentName));
                    sqlParams1.Add(new SqlParameter("@DepartmentName", search.DepartmentName));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY D.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("DepartmentName"))
                        {
                            orderByClause = "\n ORDER BY D.DepartmentName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("DepartmentName"))
                        {
                            orderByClause = "\n ORDER BY D.DepartmentName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(D.DepartmentId) FROM Department D "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
	                         D.DepartmentId
                            ,D.DepartmentName
                            ,D.CreatedById
                            ,D.CreatedBy
                            ,D.CreatedDate
                            ,D.ModifiedById
                            ,D.ModifiedBy
                            ,D.ModifiedDate
                            ,D.IsActive
                            FROM dbo.Department D"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<Department>(sql, search);
                var DepartmentsList = data.ToList<Department>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<Department>, int>(DepartmentsList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        [HttpPost("UpsertDepartmentSubCategory")]
        public async Task<IActionResult> UpsertDepartmentSubCategory(DepartmentSubCategory subcategory)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (subcategory != null)
                {
                    if (subcategory.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO DepartmentSubCategories (
                                                SubCategoryName,
                                                DepartmentId,
                                                DepartmentName,
                                                CreatedById,
                                                CreatedBy,
                                                CreatedDate,
                                                ModifiedById,
                                                ModifiedBy,
                                                ModifiedDate,
                                                IsActive,
                                                IsActiveApproved
                                            )
                                            VALUES (
                                                @SubCategoryName,
                                                @DepartmentId,
                                                @DepartmentName,
                                                @CreatedById,
                                                @CreatedBy,
                                                @CreatedDate,
                                                @ModifiedById,
                                                @ModifiedBy,
                                                @ModifiedDate,
                                                @IsActive,
                                                @IsActiveApproved
                                            );";

                        subcategory.CreatedById = loggedinUserId;
                        subcategory.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        subcategory.CreatedDate = DateTime.UtcNow;
                        subcategory.ModifiedById = loggedinUserId;
                        subcategory.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        subcategory.ModifiedDate = DateTime.UtcNow;
                        subcategory.IsActive = true;
                        subcategory.IsActiveApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, subcategory);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE DepartmentSubCategories
                                            SET
                                                SubCategoryName = @SubCategoryName,
                                                DepartmentId = @DepartmentId,
                                                DepartmentName = @DepartmentName,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate,
                                                IsActive = @IsActive
                                            WHERE
                                                Id = @Id;";

                        subcategory.ModifiedById = loggedinUserId;
                        subcategory.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        subcategory.ModifiedDate = DateTime.UtcNow;
                        subcategory.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, subcategory);
                        connection.Close();
                    }


                }

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    //Message = "Site created successfully!",
                    //Data = new { Id = productId }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);

                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpGet("GetDepartmentSubCategoryById")]
        public async Task<ActionResult> GetDepartmentSubCategoryById(int Id)
        {
            try
            {
                var departmentquery = "Select * from DepartmentSubCategories Where Id = @Id";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var departmentdata = await connection.QueryFirstOrDefaultAsync<DepartmentSubCategory>(departmentquery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Department = departmentdata
                });
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

        [HttpPost("DeleteDepartmentSubCategoryById")]
        public async Task<ActionResult> DeleteDepartmentSubCategoryById(int Id,int DepartmentId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var departmentquery = @"Update DepartmentSubCategories SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id and DepartmentId = @DepartmentId";
                var paramas = new { Id = Id, DepartmentId = DepartmentId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);
                connection.Close();


                var query = "Select * from DepartmentSubCategories where Id = @Id";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var deptsubCategoryData = await connection.QueryFirstOrDefaultAsync<DepartmentSubCategory>(query, paramas);
                _connection.Close();

                await SendSubCategoryDeletionEmail(deptsubCategoryData);


                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
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


        [HttpPost("ApproveDeleteDepartmentSubCategoryById")]
        public async Task<ActionResult> ApproveDeleteDepartmentSubCategoryById(int Id,int DepartmentId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"Select E.Email,E.FirstName, E.LastName from DepartmentSubCategories D 
                            JOIN USERS U ON U.UserId = D.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where D.Id = @Id and D.DepartmentId = @DepartmentId";

                var _params = new { Id = Id,DepartmentId = DepartmentId };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();

                var departmentquery = @"Update DepartmentSubCategories SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where Id = @Id and DepartmentId = @DepartmentId";
                var paramas = new { Id = Id, DepartmentId = DepartmentId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);
                connection.Close();


                var _query = "Select * from DepartmentSubCategories where Id = @Id and DepartmentId = @DepartmentId";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var deptsubcatdata = await connection.QueryFirstOrDefaultAsync<DepartmentSubCategory>(_query, paramas);
                __connection.Close();

                await SendDepartmentSubCatDeletionApprovalEmail(employee, deptsubcatdata);



                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
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

        [HttpPost("GetDepartmentsSubCategories")]
        public async Task<IActionResult> GetDepartmentsSubCategories(DepartmentsSubCategoriesFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterDepartmentsSubCategoriesData(filters);
                dtr.iTotalRecords = tuple.Item2;
                dtr.iTotalDisplayRecords = tuple.Item2;
                dtr.aaData = tuple.Item1;
                return Ok(dtr);

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
        private async Task<Tuple<List<DepartmentSubCategory>, int>> FilterDepartmentsSubCategoriesData(DepartmentsSubCategoriesFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where DS.IsActiveApproved = 0";

                search.IsActive = true;


                if (search.DepartmentId != null)
                {
                    whereClause += "\n AND DS.DepartmentId like '%'+@DepartmentId+'%'";
                    sqlParams.Add(new SqlParameter("@DepartmentId", search.SubCategoryName));
                    sqlParams1.Add(new SqlParameter("@DepartmentId", search.SubCategoryName));
                }

                if (!string.IsNullOrEmpty(search.SubCategoryName))
                {
                    whereClause += "\n AND DS.SubCategoryName like '%'+@SubCategoryName+'%'";
                    sqlParams.Add(new SqlParameter("@SubCategoryName", search.SubCategoryName));
                    sqlParams1.Add(new SqlParameter("@SubCategoryName", search.SubCategoryName));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY DS.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SubCategoryName"))
                        {
                            orderByClause = "\n ORDER BY DS.SubCategoryName asc";
                        }                
                        if (search.SortCol.Equals("DepartmentName"))
                        {
                            orderByClause = "\n ORDER BY DS.DepartmentName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SubCategoryName"))
                        {
                            orderByClause = "\n ORDER BY DS.SubCategoryName desc";
                        }
                        if (search.SortCol.Equals("DepartmentName"))
                        {
                            orderByClause = "\n ORDER BY DS.DepartmentName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(DS.Id) FROM DepartmentSubCategories DS "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT * 
                            FROM dbo.DepartmentSubCategories DS"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<DepartmentSubCategory>(sql, search);
                var DepartmentsSubCategoriesList = data.ToList<DepartmentSubCategory>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<DepartmentSubCategory>, int>(DepartmentsSubCategoriesList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        private async Task SendDeletionEmail(Department dept)
        {
            try
            {
                // Get employee email
                var emailAdminQuery = @"SELECT E.FirstName,E.LastName,E.EMail FROM Employee E 
                                        JOIN USERS U ON U.EmployeeCode = E.EmployeeCode 
                                        JOIN Roles R ON R.RoleId = U.UserRoles  
                                        WHERE R.RoleName like '%Admin%' AND U.IsActive = 1 AND E.Status = 1";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var adminDetails = await _connection.QueryAsync<Employee>(emailAdminQuery, null);
                var adminslist = adminDetails.ToList<Employee>();
                _connection.Close();
                Dictionary<string, string> toEmails = new Dictionary<string, string>();

                foreach (var admin in adminslist)
                {
                    toEmails.Add(admin.FirstName + " " + admin.LastName, admin.Email);
                }
                if (adminslist != null)
                {
                    var subject = $"Department Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{dept.ModifiedBy} has requested to delete the department named {dept.DepartmentName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Manage Departments to approve the deletion request.</p></br></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailMultipleAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: toEmails,
                        cc: null,
                        bcc: null,
                        subject: subject,
                        body: body,
                        AttachmentsPaths: null,
                        IsBodyHtml: true,
                        delimitter: ';'
                    );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }

        private async Task SendSubCategoryDeletionEmail(DepartmentSubCategory deptsubcat)
        {
            try
            {
                // Get employee email
                var emailAdminQuery = @"SELECT E.FirstName,E.LastName,E.EMail FROM Employee E 
                                        JOIN USERS U ON U.EmployeeCode = E.EmployeeCode 
                                        JOIN Roles R ON R.RoleId = U.UserRoles  
                                        WHERE R.RoleName like '%Admin%' AND U.IsActive = 1 AND E.Status = 1";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var adminDetails = await _connection.QueryAsync<Employee>(emailAdminQuery, null);
                var adminslist = adminDetails.ToList<Employee>();
                _connection.Close();
                Dictionary<string, string> toEmails = new Dictionary<string, string>();

                foreach (var admin in adminslist)
                {
                    toEmails.Add(admin.FirstName + " " + admin.LastName, admin.Email);
                }
                if (adminslist != null)
                {
                    var subject = $"Department Sub-Category Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{deptsubcat.ModifiedBy} has requested to delete the department sub-category named {deptsubcat.SubCategoryName} against department name {deptsubcat.DepartmentName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Manage Departments to approve the deletion request.</p></br></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailMultipleAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: toEmails,
                        cc: null,
                        bcc: null,
                        subject: subject,
                        body: body,
                        AttachmentsPaths: null,
                        IsBodyHtml: true,
                        delimitter: ';'
                    );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }


        private async Task SendDepartmentDeletionApprovalEmail(Employee employee, Department dept)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Department Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{dept.ModifiedBy} has approved the request to delete the department named {dept.DepartmentName} and it is deleted successfully from the system.</p></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: employee.Email,
                        toname: employee.FirstName + " " + employee.LastName,
                        cc: null,
                        bcc: null,
                        subject: subject,
                        body: body,
                        AttachmentsPaths: null,
                        IsBodyHtml: true,
                        delimitter: ';'
                    );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }

        private async Task SendDepartmentSubCatDeletionApprovalEmail(Employee employee, DepartmentSubCategory deptsubcat)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Department Sub-Category Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{deptsubcat.ModifiedBy} has approved the request to delete the department sub-category named {deptsubcat.SubCategoryName} against department named {deptsubcat.DepartmentName} and it is deleted successfully from the system.</p></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: employee.Email,
                        toname: employee.FirstName + " " + employee.LastName,
                        cc: null,
                        bcc: null,
                        subject: subject,
                        body: body,
                        AttachmentsPaths: null,
                        IsBodyHtml: true,
                        delimitter: ';'
                    );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }

    }
}
