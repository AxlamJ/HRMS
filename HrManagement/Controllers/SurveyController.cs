using DocumentFormat.OpenXml.ExtendedProperties;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class SurveyController : Controller
    {
        private readonly Common _common;

        public SurveyController(Common common)
        {
            _common = common;
        }

        public IActionResult CreateSurvey()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRoles = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = new List<Site>();
                if (!string.IsNullOrEmpty(UserSites))
                {
                    EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);
                }
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
                    Sites = Sites,
                    Employees = Employees
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult ManageSurveys()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();

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
                    Sites = Sites,
                    Managers = Managers,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus,
                    Employees = Employees
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult MySurveys()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
                var EmployeeLevels = _common.GetAllAsync<EmployeeLevels>("EmployeeLevels", HttpContext).GetAwaiter().GetResult();
                var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
                var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();


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
                    Sites = Sites,
                    Managers = Managers,
                    EmployeeLevels = EmployeeLevels,
                    EmployeePosition = EmployeePosition,
                    EmployeeStatus = EmployeeStatus,
                    Employees = Employees
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult EmployeeSurvey()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();

                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var LoggedInEmployeeCode = _common.GetEmployeeCodeByUserId<int>(loggedinUserId).GetAwaiter().GetResult();
                ViewBag.LoggedInEmployeeCode = LoggedInEmployeeCode;


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
                    Sites = Sites,
                    Employees = Employees
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
