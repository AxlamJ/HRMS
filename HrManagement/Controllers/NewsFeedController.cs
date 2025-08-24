using DocumentFormat.OpenXml.ExtendedProperties;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class NewsFeedController : Controller
    {

        private readonly Common _common;

        public NewsFeedController(Common common)
        {
            _common = common;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult NewsFeedManagement()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult NewsFeedForm()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
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
                    Employees = Employees
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }     
        
        public IActionResult News()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
