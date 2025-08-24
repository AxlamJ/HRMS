using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Controllers
{
    public class LearningProgressController : Controller
    {
        public IActionResult Index(string? id = null)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
