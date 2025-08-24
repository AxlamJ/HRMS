using DocumentFormat.OpenXml.ExtendedProperties;
using HrManagement.Dto;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class AssignTrainingController : Controller
    {
        private readonly Common _common;

        public AssignTrainingController(Common common)
        {
            _common = common;
        }
        public IActionResult Permissions()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var AssignTrainingByRole = _common.GetAllAsync<TrainingPermissionDto>("GetTrainingWithPermission", HttpContext).GetAwaiter().GetResult();

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
                    Trainings=Trainings,
                    AssignTrainingByRole = AssignTrainingByRole,
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult PermissionsByDepratment()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {

                var Trainings = _common.GetAllAsync<Training>("Trainings", HttpContext).GetAwaiter().GetResult();

                //visiable to Department
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
               ///
                
                //Child
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
               
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();



                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Employees = new List<Employee>();

                    Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
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
                    Trainings = Trainings
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
