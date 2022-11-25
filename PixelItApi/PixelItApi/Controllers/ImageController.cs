using Microsoft.AspNetCore.Mvc;

namespace PixelItApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        public ImageController()
        {

        }

        [HttpGet()]
        public async Task<IActionResult> GetEditors()
        {
            try
            {
                var response = await this.service.GetEditorsAsync();
                return this.Ok(response);
            }
            catch (Exception ex)
            {
                return this.Problem(ex.ToString());
            }
        }
    }
}
