using Microsoft.AspNetCore.SignalR;
using PixelIt.Contracts;

namespace PixelItWatchdogApi.HubConfig;

public class ImageHub : Hub
{
    public async Task BroadcastImageData(Image data) =>
        await Clients.All.SendAsync("broadcastimagedata", data);
}