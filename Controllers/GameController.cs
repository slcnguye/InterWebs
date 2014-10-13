using System.Linq;
using System.Web.Mvc;
using InterWebs.Domain.Model;
namespace InterWebs.Controllers
{
    public class GameController : Controller
    {
        public ActionResult GameTable()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();   
        }
    }
}