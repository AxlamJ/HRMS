using HrManagement.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly Common _common;

        public ReportsController(Common common)
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
        public IActionResult Sites()
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
        public IActionResult EmployeeGender()
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
        public IActionResult EmployeeAge()
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
        public IActionResult Departments()
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
        public IActionResult HeadCounts()
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
        public IActionResult HiredVsLeft()
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
        public IActionResult BirthdayList()
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
        public IActionResult TerminationDismissalReasons()
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
        public IActionResult TerminationDismissalTypes()
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
        public IActionResult EmployeeLevel()
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
        public IActionResult WorkDuration()
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
