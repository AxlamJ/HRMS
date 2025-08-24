using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly Common _common;

        public EmployeeController(Common common)
        {
            _common = common;
        }
        public IActionResult ManageEmployees()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
                var TerminationDismissalReason = _common.GetAllAsync<TerminationDismissalReason>("TerminationDismissalReason", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRole = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRole) && UserRole.ToLower().IndexOf("admin") < 0 && UserRole.ToLower().IndexOf("super admin") < 0)
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
                    Managers = Managers,
                    Roles = Roles,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus,
                    TerminationDismissalReason = TerminationDismissalReason
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult AddUpdateEmployee()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Managers = new List<Employee>();

                    Managers = _common.GetAllAsync<Employee>("UserSiteManagers", HttpContext).GetAwaiter().GetResult();
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
                    Managers = Managers,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult MyProfile()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = new List<Site>();
                if (!string.IsNullOrEmpty(UserSites))
                {
                    EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);
                }

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Managers = new List<Employee>();

                    Managers = _common.GetAllAsync<Employee>("UserSiteManagers", HttpContext).GetAwaiter().GetResult();
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
                    Managers = Managers,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus,
                    Roles = Roles
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult EmployeeDetails()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var DepartmentSubCategories = _common.GetAllAsync<DepartmentSubCategory>("DepartmentSubCategories", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
                var Roles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                {
                    Managers = new List<Employee>();

                    Managers = _common.GetAllAsync<Employee>("UserSiteManagers", HttpContext).GetAwaiter().GetResult();
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
                    Managers = Managers,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus,
                    Roles = Roles
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }

    public class Site
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
