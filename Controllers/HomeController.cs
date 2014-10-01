using System.Web.Mvc;

namespace InterWebs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            if (User.Identity.Name != "sang")
            {
                return RedirectToAction("Index", "Home");          
            }

            return View();   
        }
    }
}