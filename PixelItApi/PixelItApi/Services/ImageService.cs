using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using PixelIt.Contracts;
using Image = PixelIt.Contracts.Image;

namespace PixelItApi.Services;

public class ImageService : IImageService
{
    private readonly IDatabaseService databaseService;
    private string connectionString;
    private const int imageParts = 4;
    public ImageService(IDatabaseService databaseService, IConfiguration configuration)
    {
        this.databaseService = databaseService;
        this.connectionString = configuration.GetConnectionString("InQueueClient");
    }

    public async Task WriteToPixelateImageQueue(Image image)
    {
        try
        {
            await using var inQueueClient = new ServiceBusClient(this.connectionString);
            var sender = inQueueClient.CreateSender("pixelitin");
            var parts = new List<string>();

            List<System.Drawing.Image> splitImages = this.SplitImages(this.CreateImage(Convert.FromBase64String(image.StringBytes)), imageParts);

            foreach (var imagePart in splitImages)
            {
                parts.Add(Convert.ToBase64String(ImageToByteArray(imagePart)));
            }
            
            var partNumber = 1;
            foreach (var part in parts)
            {
                var identificator = Guid.NewGuid();
                var messageJson = JsonSerializer.Serialize(new ImagePart
                {
                    ImageId = image.Id,
                    Identificator = identificator,
                    StringBytes = part,
                    PartNumber = partNumber,
                    TotalPart = 3
                });
                await sender.SendMessageAsync(new ServiceBusMessage(messageJson));

                partNumber++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private byte[] ImageToByteArray(System.Drawing.Image image)
    {
        byte[] temp = null;
        using (MemoryStream m = new MemoryStream())
        {
            image.Save(m, ImageFormat.Png);
            temp = m.ToArray();
        }

        return temp;
    } 

    public Task SaveImage(Image image)
    {
       return this.databaseService.SaveImage(image);
    }

    public Task<List<Image>> GetImages()
    {
        return this.databaseService.GetImages();
    }

    private System.Drawing.Image CreateImage(byte[] imageData)
    {
        System.Drawing.Image image;
        using (var inStream = new MemoryStream())
        {
            inStream.Write(imageData, 0, imageData.Length);

            image = Bitmap.FromStream(inStream);
        }

        return image;
    }

    private List<System.Drawing.Image> SplitImages(System.Drawing.Image image, int partCounts)
    {
        List<System.Drawing.Image> list = new List<System.Drawing.Image>();
        
        for( int i = 0; i < 1; i++){
            for( int j = 0; j < partCounts; j++)
            {
                var bitmap = new Bitmap(image.Width, image.Height / partCounts);
                
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage( image, new Rectangle(0,0,image.Width,image.Height / partCounts), new Rectangle(i, j*(image.Height / partCounts),image.Width,(image.Height / partCounts)), GraphicsUnit.Pixel);
                graphics.Dispose();
                list.Add(bitmap);
            }
        }
        
        return list;
    }
}