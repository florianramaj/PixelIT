using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using PixelIt.Contracts;
using PixelItWatchdogApi.HubConfig;

namespace PixelItWatchdogApi.Services;

public class WatchdogService : IWatchdogService
{
    private bool isRunning;
    private readonly IHubContext<ImageHub> hub;
    private readonly string connectionString;
    public WatchdogService(IHubContext<ImageHub> hub, IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("OutQueueClient"); 
        this.isRunning = false;
        this.hub = hub;
    }

    public void StartListening()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;

        Task.Run(this.Worker);
    }

    private async Task Worker()
    {
        try
        {
            await using var outQueueClient = new ServiceBusClient(this.connectionString);

            ServiceBusReceiver receiver = outQueueClient.CreateReceiver("pixelitout");
            Console.WriteLine($"[Listener started|{DateTime.Now}]");
            do
            {
                ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
                Console.WriteLine($"[InputQueue|{DateTime.Now}]");

                var imagePart = receivedMessage.Body.ToObjectFromJson<ImagePart>();

                await this.hub.Clients.All.SendAsync("ImageData", imagePart);
                receiver.CompleteMessageAsync(receivedMessage);
                Console.WriteLine($"[OutputQueue|{DateTime.Now}]: {imagePart.ToString()}");
            } while (!receiver.IsClosed);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            this.isRunning = false;
            this.StartListening();
        }
        
    }
}