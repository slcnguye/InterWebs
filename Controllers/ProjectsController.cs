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
            ViewBag.ChatMessages = chatMessageRepository.GetAll().ToList();

            return View();   
        }

        [Route("projects/chat")]
        public ActionResult Chat()
        {
            var usernameCookie = HttpContext.Request.Cookies["username"];

            ViewBag.UserName = usernameCookie == null ? null : usernameCookie.Value; 
            ViewBag.ChatMessages = chatMessageRepository.GetAll().ToList();
            return View();
        }
        
        [Route("projects")]
        public ActionResult Projects()
        {
            return View();
        }

        [HttpPost]
        public void StoreChatMessage(string message)
        {
            var usernameCookie = HttpContext.Request.Cookies["username"];
            var username = usernameCookie == null ? null : usernameCookie.Value;
            if (String.IsNullOrWhiteSpace(username))
            {
                return;
            }

            var chatMessage = new ChatMessage
            {
                Message = message,
                User = username
            };

            chatMessageRepository.Insert(chatMessage);
        }

        [HttpPost]
        public ActionResult SetUsername(string username)
        {
            HttpContext.Response.Cookies.Add(new HttpCookie("username")
            {
                Value = username
            });

            return PartialView("_NavigationMenu");
        }
    }
}