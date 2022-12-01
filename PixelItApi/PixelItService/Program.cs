using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using PixelIt.Contracts;
using PixelItService;

var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true);

var config = builder.Build();

const int CellSize = 16;

var pixelCalculator = new PixelCalulcator(CellSize);

await using var inQueueClient = new ServiceBusClient(config.GetConnectionString("InQueueClient"));

ServiceBusReceiver receiver = inQueueClient.CreateReceiver("pixelitin");
var sender = inQueueClient.CreateSender("pixelitout");
do
{
    try
    {
        ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
        Console.WriteLine($"[InputQueue|{DateTime.Now}]");
        var newImagePart = pixelCalculator.PixelateImagePart(receivedMessage.Body.ToObjectFromJson<ImagePart>());
    
        var messageJson = JsonSerializer.Serialize(new ImagePart
        {
            ImageId = newImagePart.ImageId,
            Identificator = newImagePart.Identificator,
            StringBytes = newImagePart.StringBytes,
            PartNumber = newImagePart.PartNumber,
            TotalPart = newImagePart.TotalPart
        });
    
        await sender.SendMessageAsync(new ServiceBusMessage(messageJson));
        receiver.CompleteMessageAsync(receivedMessage);
        Console.WriteLine($"[OutputQueue|{DateTime.Now}]: {messageJson}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

} while (true);