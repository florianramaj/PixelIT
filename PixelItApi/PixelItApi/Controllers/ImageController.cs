using Microsoft.AspNetCore.Mvc;
using PixelIt.Contracts;
using PixelItApi.Services;

namespace PixelItApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IImageService service;

        public ImageController(IImageService imageService)
        {
            this.service = imageService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetImages()
        {
            try
            {
                var response = await this.service.GetImages();
                return this.Ok(response);
            }
            catch (Exception ex)
            {
                return this.Problem(ex.ToString());
            }
        }
        
        [HttpPost()]
        public async Task<IActionResult> SaveImage([FromBody] Image image)
        {
            try
            {
                await this.service.SaveImage(image);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                return this.Problem(ex.ToString());
            }
           
        }
        
        [HttpPost("/PixilateImage")]
        public async Task<IActionResult> PixilateImage([FromBody] Image image)
        {
            try
            {
                await this.service.WriteToPixelateImageQueue(image);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                return this.Problem(ex.ToString());
            }
           
        }
    }
}
