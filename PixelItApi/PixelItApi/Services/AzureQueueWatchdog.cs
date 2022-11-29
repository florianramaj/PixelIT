using Azure.Storage.Queues;
using PixelIt.Contracts;

namespace PixelItApi.Services;

public class AzureQueueWatchdog : IAzureQueueWatchdog
{
    private readonly QueueClient outputQueueClient;

    private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=pixelit;AccountKey=pCfLoUpdc2ku/a1XDfOfT4j8RWZNfVQjqwK/iD2HP08cxQK7WP2twUcH1bhXY0XRIUPdNzDGkoiX+AStCAPTLQ==;EndpointSuffix=core.windows.net";

    private bool isRunning;
    
    public AzureQueueWatchdog()
    {
        this.outputQueueClient = new QueueClient(ConnectionString, "pixelitoutputqueue");
        this.isRunning = false;
    }
    
    public void Start()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;
    }

    public List<ImagePart> GetImageParts()
    {
        throw new NotImplementedException();
    }
    
    
}