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

        public string GetUserName()
        {
            var usernameCookie = HttpContext.Request.Cookies["username"];
            return usernameCookie == null ? null : usernameCookie.Value;
        }
    }
}