using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace InterWebs.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Game()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = User.Identity.Name;
            ViewBag.Cards = Directory.GetFiles(Server.MapPath(@"~\Content\Images\Playing Cards")).Select(Path.GetFileName);
            ViewBag.CardPath = "/InterWebs/Content/Images/Playing Cards/";

            return View();   
        }
    }
}