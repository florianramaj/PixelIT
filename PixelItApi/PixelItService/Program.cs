using System.Text.Json;
using Azure.Messaging.ServiceBus;
using PixelIt.Contracts;
using PixelItService;

const string ConnectionString = "Endpoint=sb://pixelit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2eJm7AvDsYDiXkEE3V8ADeZhnGJ41FRJCn2A4eG0nlM=";
const int CellSize = 16;

var pixelCalculator = new PixelCalulcator(CellSize);

await using var inQueueClient = new ServiceBusClient(ConnectionString);
await using var outQueueClient = new ServiceBusClient(ConnectionString);

ServiceBusReceiver receiver = inQueueClient.CreateReceiver("pixelitin");
var sender = inQueueClient.CreateSender("pixelitout");
do
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
} while (!receiver.IsClosed);