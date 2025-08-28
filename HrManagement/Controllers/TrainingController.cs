using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Dto;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;


namespace HrManagement.Controllers
{
    public class TrainingController : Controller
    {
        private readonly Common _common;
        public TrainingController(Common common)
        {
            _common = common;
        }

        [Route("Courses/Product_blueprints")]
        public IActionResult Productblueprints()
        {
            return View();
        }

        [Route("traning/course_detail/{id}")]
        public IActionResult CourseDetail(int id)
        {
            ViewBag.TrainingId = id;


            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Trainings = _common.GetAllAsync<Training>("Trainings", HttpContext).GetAwaiter().GetResult();
                var AssignTrainingByRole = _common.GetAllAsync<Training>("GetTrainingWithPermission", HttpContext).GetAwaiter().GetResult();

                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();



                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Sites = new List<Sites>();
                    foreach (var site in EmployeeSites)
                    {

                        Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    }
                }

                ViewBag.DropDownData = new
                {
                    Departments = Departments,
                    Roles = Roles,
                    Sites = Sites,
                    Trainings = Trainings,
                    AssignTrainingByRole = AssignTrainingByRole,
                    Employees = Employees,
                    DepartmentSubCategories = DepartmentSubCategories,
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [Route("Courses/detail")]
        public IActionResult detail(int? Id)
        {
            ViewBag.Id = Id;
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Trainings = _common.GetAllAsync<Training>("Trainings", HttpContext).GetAwaiter().GetResult();

                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Sites = new List<Sites>();
                    foreach (var site in EmployeeSites)
                    {

                        Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    }
                }

                ViewBag.DropDownData = new
                {
                    Departments = Departments,
                    Roles = Roles,
                    Sites = Sites,
                    Trainings = Trainings,
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [Route("Training/Edit_Outline/{id}")]
        public IActionResult Edit_outline(string? Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        [Route("Training/Post_Details/{id}")]
        public IActionResult Post_Details(string? Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        [Route("Training/Tarining_Assesment/{id}")]
        public IActionResult Tarining_Assesment(string? Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        [Route("Training/Certificate")]
        public IActionResult Certificate()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            return View();
        }

        [Route("Training/TrainingReport")]
        public IActionResult TrainingReport()
        {
           // ViewBag.TrainingId = id;


            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Trainings = _common.GetAllAsync<TrainingsReports>("TrainingsReports", HttpContext).GetAwaiter().GetResult();
                var LessonProgress = _common.GetAllAsync<UserLessonProgressReport>("UserLessonProgressReport", HttpContext).GetAwaiter().GetResult();
                var QuizResult = _common.GetAllAsync<QuizReports>("UserQuizAttempt", HttpContext).GetAwaiter().GetResult();
                var Structure = _common.GetAllAsync<Structure>("TrainingStructure", HttpContext).GetAwaiter().GetResult();
                var Courses = _common.GetAllAsync<Courses>("TrainingCategory", HttpContext).GetAwaiter().GetResult();


                var AssignTrainingByRole = _common.GetAllAsync<Training>("GetTrainingWithPermission", HttpContext).GetAwaiter().GetResult();
                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();
                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Sites = new List<Sites>();
                    foreach (var site in EmployeeSites)
                    {

                        Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    }
                }

                ViewBag.DropDownData = new
                {
                    Departments = Departments,
                    DepartmentSubCategories = DepartmentSubCategories,
                    Sites = Sites,
                    Employees = Employees,
                    Trainings = Trainings,
                    CoursesQuizResult = QuizResult,
                    Courses = Courses,
                    Semester = Structure,
                    CoursesProgress = LessonProgress,
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

    }
}
