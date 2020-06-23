using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Hubs;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly ChatsManager _chatsManager;

        public ChatsController(Model context, 
            IMapper mapper, 
            IHubContext<ChatHub> hub,
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _hub = hub;
            _chatsManager = new ChatsManager(context, mapper, userManager, appSettings);
        }
    }
}
