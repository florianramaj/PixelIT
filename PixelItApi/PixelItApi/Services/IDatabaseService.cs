using PixelIt.Contracts;

namespace PixelItApi.Services
{
    public interface IDatabaseService
    {
        Task SaveImage(Image image);

        Task<Image> GetImages();
    }
}
