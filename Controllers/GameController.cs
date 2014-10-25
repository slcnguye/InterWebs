using System.IO;
using System.Linq;
using System.Web.Mvc;
using InterWebs.Models.Game;
using Newtonsoft.Json;

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

            if (Card.CardsSource == null)
            {
                var file = System.IO.File.ReadAllText(Server.MapPath(@"~\Content\CardValueMapper.json"));
                Card.CardsValue = JsonConvert.DeserializeObject(file, new JsonSerializerSettings());
                Card.CardsSource = Directory.GetFiles(Server.MapPath(@"~\Content\Images\Playing Cards")).Select(Path.GetFileName).ToList();
            }

            ViewBag.UserName = User.Identity.Name;
            ViewBag.Cards = Card.CardsSource;
            ViewBag.CardPath = "/InterWebs/Content/Images/Playing Cards/";
            ViewBag.BackCardPath = "/InterWebs/Content/Images/Playing Cards Back/Card_back.svg";

            return View();   
        }
    }
}