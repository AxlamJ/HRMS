using DocumentFormat.OpenXml.InkML;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class ScheduleManagementController : Controller
    {
        private readonly Common _common;
        public ScheduleManagementController(Common common)
        {
            _common = common;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRole = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRole) && UserRole.ToLower().IndexOf("admin") < 0 && UserRole.ToLower().IndexOf("super admin") < 0)
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
                    Employees = Employees,
                    Sites = Sites
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
