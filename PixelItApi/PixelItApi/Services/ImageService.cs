using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using Azure.Storage.Queues;
using PixelIt.Contracts;
using Image = PixelIt.Contracts.Image;

namespace PixelItApi.Services;

public class ImageService : IImageService
{
    private readonly QueueClient inQueueClient;
    private readonly QueueClient outputQueueClient;
    private readonly IDatabaseService databaseService;
    private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=pixelit;AccountKey=pCfLoUpdc2ku/a1XDfOfT4j8RWZNfVQjqwK/iD2HP08cxQK7WP2twUcH1bhXY0XRIUPdNzDGkoiX+AStCAPTLQ==;EndpointSuffix=core.windows.net";
    private const int imageParts = 4;
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

            List<System.Drawing.Image> splitImages = this.SplitImages(this.CreateImage(Convert.FromBase64String(image.StringBytes)), imageParts);

            foreach (var imagePart in splitImages)
            {
                parts.Add(Convert.ToBase64String(ImageToByteArray(imagePart)));
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

    private static IEnumerable<string> Split(string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
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
    
    private static System.Drawing.Image cropImage(System.Drawing.Image img, Rectangle cropArea)
    {
        Bitmap bmpImage = new Bitmap(img);
        Bitmap bmpCrop = bmpImage.Clone(cropArea, System.Drawing.Imaging.PixelFormat.DontCare);
        return bmpCrop;
    }
}