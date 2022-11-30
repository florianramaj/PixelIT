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
    const string ConnectionString = "Endpoint=sb://pixelit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2eJm7AvDsYDiXkEE3V8ADeZhnGJ41FRJCn2A4eG0nlM=";
    public WatchdogService(IHubContext<ImageHub> hub)
    {
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
            await using var outQueueClient = new ServiceBusClient(ConnectionString);

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