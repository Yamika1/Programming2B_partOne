using Microsoft.AspNetCore.Mvc;

namespace Prog2Bpartone.Controllers
{
    public class MonthlyClaim : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
     
        public IActionResult Review()
        {
            return View();
        }
        public IActionResult Documents()
        {
            return View();
        }
    }
}
