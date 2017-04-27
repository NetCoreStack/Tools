using Hisar.Component.Guideline.Models;
using NetCoreStack.Hisar;
using Microsoft.AspNetCore.Mvc;
using Sample.ClassLibrary;

namespace Hisar.Component.Guideline.Controllers
{
    [HisarRoute(nameof(Guideline))]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var externalLibrary = new ExternalLibrary();
            ViewBag.ExternalLibrary = externalLibrary.Name;
            return View();
        }
        
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(GuidelineViewModel model)
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrationJson([FromBody]GuidelineViewModel model)
        {
            return Json(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}