using ChatGptApi.Interfaces;
using ChatGptApi.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> Create()
        {
            var model = new ImageProperties { Prompt = "A cute baby sea otter", Number = 1, Size = "1024x1024" };

            return Ok(await _imageLogic.CreateImage(model));
        }
    }
}
