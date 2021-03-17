using Microsoft.AspNetCore.Mvc;

namespace CrilieTechReviewsBlog.Controllers
{
    public class ErrorController : Controller
    {
        
        [HttpGet]
        public IActionResult ResourceNotFound(string errorName ="")
        {
            ViewBag.ErrorName = errorName;
            return View();
        }

        [HttpGet]
        public IActionResult SomethingWentWrong() => View();
    }
}
