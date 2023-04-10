using ChatGptApi.Interfaces;
using ChatGptApi.Logic.Chat;
using ChatGptApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatGptApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : Controller
    {
        private readonly IImageLogic _imageLogic;
        public ImagesController(IImageLogic imageLogic)
        {
            _imageLogic = imageLogic;
        }

        [HttpPost(Name = "Create")]
        public async Task<ActionResult> Create(string text)
        {
            try
            {
                return Ok(await _imageLogic.CreateImage(text));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
