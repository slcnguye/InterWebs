using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterWebs.Domain.Model;
using InterWebs.Domain.Repository;
using InterWebs.Models.Game;
using Newtonsoft.Json;

namespace InterWebs.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IPersistenceOrientedRepository<ChatMessage> chatMessageRepository;

        public ProjectsController(IPersistenceOrientedRepository<ChatMessage> chatMessageRepository)
        {
            this.chatMessageRepository = chatMessageRepository;
        }

        [Route("projects/game")]
        public ActionResult Game()
        {
            if (Card.CardsSource == null)
            {
                var file = System.IO.File.ReadAllText(Server.MapPath(@"~\Content\CardValueMapper.json"));
                Card.CardsValue = JsonConvert.DeserializeObject(file, new JsonSerializerSettings());
                Card.CardsSource = Directory.GetFiles(Server.MapPath(@"~\Content\Images\Playing Cards")).Select(Path.GetFileName).ToList();
            }

            var usernameCookie = HttpContext.Request.Cookies["username"];

            ViewBag.UserName = usernameCookie == null? null : usernameCookie.Value;
            ViewBag.Cards = Card.CardsSource;
            ViewBag.CardPath = Url.Content("~/Content/Images/Playing Cards/");
            ViewBag.BackCardPath = Url.Content("~/Content/Images/Playing Cards Back/Card_back.svg");
            ViewBag.BlankCardPath = Url.Content("~/Content/Images/Playing Cards Back/Card_blank.svg");
            ViewBag.ChatMessages = chatMessageRepository.GetAll(x => x.ChatName == "All").ToList();

            return View();   
        }

        [Route("projects/chat")]
        public ActionResult Chat()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.ChatMessages = chatMessageRepository.GetAll(x => x.ChatName == "All").ToList();
            return View();
        }

        [HttpPost]
        public void StoreChatMessage(string message)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return;
            }

            var chatMessage = new ChatMessage
            {
                ChatName = "All",
                Message = message,
                User = User.Identity.Name
            };

            chatMessageRepository.Insert(chatMessage);
        }

        [HttpPost]
        public void SetUsername(string username)
        {
            HttpContext.Response.Cookies.Add(new HttpCookie("username")
            {
                Value = username
            });
        }
    }
}