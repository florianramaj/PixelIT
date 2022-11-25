using PixelIt.Contracts;

namespace PixelItApi.Services;

public interface IImageService
{
    Task SaveImage(Image image);
    
    Task<Image> GetImages();
}