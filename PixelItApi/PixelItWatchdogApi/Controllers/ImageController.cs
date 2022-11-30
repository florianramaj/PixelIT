using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PixelIt.Contracts;
using PixelItWatchdogApi.HubConfig;
using PixelItWatchdogApi.Services;

namespace PixelItWatchdogApi.Controllers;

[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IHubContext<ImageHub> _hub;
    private readonly IWatchdogService watchdogService;

    public ImageController(IWatchdogService watchdogService)
    {
        this.watchdogService = watchdogService;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        this.watchdogService.StartListening();
        return Ok(new { Message = "Watchdog started." });
    }
    
    
}