using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Controllers
{
    public class TrainingSessionController : Controller
    {
        public IActionResult WatchLesson(int? id = null)
        {
            if (id == null)
            {
                return BadRequest("Id is required");
            }
            ViewBag.id = id;
            return View();
        }

        [HttpGet]
        public IActionResult Assessment(int? id = null, int? lessonId = null, string title = null)
        {
            ViewBag.id = id;
            ViewBag.title = title;
            ViewBag.lessonId = lessonId;
            return View();
        }

        public IActionResult Quiz(int? id = null, int? lessonId = null)
        {
            ViewBag.id = id;
         
            return View();

        }

        [HttpGet]
        public IActionResult Lesson(int? id = null,int? lessonId=null)
        {
            ViewBag.id = id;
            ViewBag.lessonId = lessonId;
            return View();

        }
    }
}
