using PixelIt.Contracts;

namespace PixelItApi.Services;

public interface IImageService
{
    Task WriteToPixelateImageQueue(Image image);
    Task SaveImage(Image image);
    
    Task<List<Image>> GetImages();
}