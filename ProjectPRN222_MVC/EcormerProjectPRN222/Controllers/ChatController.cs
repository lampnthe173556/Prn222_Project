using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project2.Hubs;

namespace EcormerProjectPRN222.Controllers
{
    public class ChatController : Controller
    {
        private readonly MyProjectClothingContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public ChatController(MyProjectClothingContext context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }
        public IActionResult Index()
        {
            
        }
    }
}
