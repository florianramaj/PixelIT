using System.Text.Json;
using Azure.Storage.Queues;
using PixelIt.Contracts;

namespace PixelItApi.Services;

public class ImageService : IImageService
{
    private readonly QueueClient inQueueClient;
    private readonly QueueClient outputQueueClient;
    private readonly IDatabaseService databaseService;
    private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=pixelit;AccountKey=pCfLoUpdc2ku/a1XDfOfT4j8RWZNfVQjqwK/iD2HP08cxQK7WP2twUcH1bhXY0XRIUPdNzDGkoiX+AStCAPTLQ==;EndpointSuffix=core.windows.net";
    private const int ServiceChunk = 30000;
    public ImageService(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
        this.inQueueClient = new QueueClient(ConnectionString, "pixelitqueue");
    }

    public async Task WriteToPixelateImageQueue(Image image)
    {
        try
        {
            await this.inQueueClient.CreateIfNotExistsAsync();
            var parts = new List<string>();
            if (image.StringBytes.Length < ServiceChunk)
            {
                parts.Add(image.StringBytes);
            }
            else
            {
                parts = Split(image.StringBytes, ServiceChunk).ToList();
            }
            var identificator = Guid.NewGuid();
            var partNumber = 1;
            foreach (var part in parts)
            {
            
                var messageJson = JsonSerializer.Serialize(new ImagePart
                {
                    Identificator = identificator,
                    StringBytes = part,
                    PartNumber = partNumber,
                    TotalPart = 3
                });
                await inQueueClient.SendMessageAsync(messageJson);

                partNumber++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task SaveImage(Image image)
    {
       return this.databaseService.SaveImage(image);
    }

    public Task<List<Image>> GetImages()
    {
        return this.databaseService.GetImages();
    }

    private static IEnumerable<string> Split(string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
    }
}