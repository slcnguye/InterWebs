using System.Web.Mvc;

namespace InterWebs.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("resume")]
        public ActionResult Resume()
        {
            return View();
        }
    }
}