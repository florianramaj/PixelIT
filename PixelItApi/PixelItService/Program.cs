// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using PixelIt.Contracts;
using PixelItService;

const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=pixelit;AccountKey=pCfLoUpdc2ku/a1XDfOfT4j8RWZNfVQjqwK/iD2HP08cxQK7WP2twUcH1bhXY0XRIUPdNzDGkoiX+AStCAPTLQ==;EndpointSuffix=core.windows.net";
const int CellSize = 16;

var pixelCalculator = new PixelCalulcator(CellSize);

var inQueueClient = new QueueClient(ConnectionString, "pixelitqueue");
var outQueueClient = new QueueClient(ConnectionString, "pixelitoutqueue");

do
{
    Response<QueueMessage> response = await inQueueClient.ReceiveMessageAsync();
    Console.WriteLine($"[InputQueue|{DateTime.Now}]");
    QueueMessage message = response.Value;
    var newImagePart = pixelCalculator.PixelateImagePart(message.Body.ToObjectFromJson<ImagePart>());
    await outQueueClient.CreateIfNotExistsAsync();
    
    var messageJson = JsonSerializer.Serialize(new ImagePart
    {
        Identificator = newImagePart.Identificator,
        StringBytes = newImagePart.StringBytes,
        PartNumber = newImagePart.PartNumber,
        TotalPart = newImagePart.TotalPart
    });
    
    await outQueueClient.SendMessageAsync(messageJson);
    Console.WriteLine($"[OutputQueue|{DateTime.Now}]: {messageJson}");
    
} while (inQueueClient.Exists());