using DocumentFormat.OpenXml.ExtendedProperties;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class SettingsController : Controller
    {
        private readonly Common _common;

        public SettingsController(Common common)
        {
            _common = common;
        }
        public IActionResult Permissions()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
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
