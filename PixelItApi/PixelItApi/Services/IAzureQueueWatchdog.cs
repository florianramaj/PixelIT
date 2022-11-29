using PixelIt.Contracts;

namespace PixelItApi.Services;

public interface IAzureQueueWatchdog
{
    void Start();
    List<ImagePart> GetImageParts();
}