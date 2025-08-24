using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HrManagement.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Common _common;
        public ProductsController(Common common)
        {
            _common = common;
        }
        public IActionResult Index()
        {
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
    }
}
