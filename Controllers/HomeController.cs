using System.Linq;
using System.Web.Mvc;
using InterWebs.Domain.Model;
using InterWebs.Domain.Repository;

namespace InterWebs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPersistenceOrientedRepository<ChatMessage> chatMessageRepository;

        public HomeController(IPersistenceOrientedRepository<ChatMessage> chatMessageRepository)
        {
            this.chatMessageRepository = chatMessageRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Resume()
        {
            return View();
        }

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
    }
}