using ChatGptApi.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatGptApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatLogic _chatLogic;
        public ChatController(IChatLogic chatLogic)
        {
            _chatLogic = chatLogic;
        }
        [HttpPost]
        public async Task<ActionResult> Chat(string chat)
        {
            try
            {
                return Ok(await _chatLogic.CreateChat(chat));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
