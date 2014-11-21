using System.IO;
using System.Linq;
using System.Web.Mvc;
using InterWebs.Domain.Model;
using InterWebs.Domain.Repository;
using InterWebs.Models.Game;
using Newtonsoft.Json;

namespace InterWebs.Controllers
{
    public class GameController : Controller
    {
        private readonly IPersistenceOrientedRepository<ChatMessage> chatMessageRepository;

        public GameController(IPersistenceOrientedRepository<ChatMessage> chatMessageRepository)
        {
            this.chatMessageRepository = chatMessageRepository;
        }


        public ActionResult Game()
        {
            if (Card.CardsSource == null)
            {
                var file = System.IO.File.ReadAllText(Server.MapPath(@"~\Content\CardValueMapper.json"));
                Card.CardsValue = JsonConvert.DeserializeObject(file, new JsonSerializerSettings());
                Card.CardsSource = Directory.GetFiles(Server.MapPath(@"~\Content\Images\Playing Cards")).Select(Path.GetFileName).ToList();
            }

            ViewBag.UserName = User.Identity.Name;
            ViewBag.Cards = Card.CardsSource;
            ViewBag.CardPath = Url.Content("~/Content/Images/Playing Cards/");
            ViewBag.BackCardPath = Url.Content("~/Content/Images/Playing Cards Back/Card_back.svg");
            ViewBag.BlankCardPath = Url.Content("~/Content/Images/Playing Cards Back/Card_blank.svg");
            ViewBag.ChatMessages = chatMessageRepository.GetAll(x => x.ChatName == "All").ToList();

            return View();   
        }

        [HttpPost]
        public void StoreChatMessage(string message)
        {
            var chatMessage = new ChatMessage
            {
                ChatName = "All",
                Message = message,
                User = User.Identity.Name
            };

            chatMessageRepository.Insert(chatMessage);
        }
    }
}