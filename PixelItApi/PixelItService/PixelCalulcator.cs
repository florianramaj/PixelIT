using System.Drawing;
using System.Drawing.Imaging;
using PixelIt.Contracts;
using Image = System.Drawing.Image;

namespace PixelItService;

public class PixelCalulcator
{
    private readonly int cellSize;

    public PixelCalulcator(int cellSize)
    {
        this.cellSize = cellSize;
    }
    
    public ImagePart PixelateImagePart(ImagePart imagePart)
    {
        var bytes = Convert.FromBase64String(imagePart.StringBytes);
        var image = CreateImage(bytes);
        var bitmap = new Bitmap(image.Width, image.Height);
    
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        // Loop through the image in 4x4 cells.
        for (var yy = 0; yy < image.Height && yy < image.Height; yy += this.cellSize)
        {
            for (var xx = 0; xx < image.Width && xx < image.Width; xx += this.cellSize)
            {
                var cellColors = new List<Color>();

                for (var y = yy; y < yy + this.cellSize && y < image.Height; y++)
                {
                    for (var x = xx; x < xx + this.cellSize && x < image.Width; x++)
                    {
                        cellColors.Add(bitmap.GetPixel(x, y));
                    }
                }

                // Get the average red, green, and blue values.
                var averageRed = cellColors.Aggregate(0, (current, color) => current + color.R) / cellColors.Count;
                var averageGreen = cellColors.Aggregate(0, (current, color) => current + color.G) / cellColors.Count;
                var averageBlue = cellColors.Aggregate(0, (current, color) => current + color.B) / cellColors.Count;

                var averageColor = Color.FromArgb(averageRed, averageGreen, averageBlue);

                for (var y = yy; y < yy + this.cellSize && y < image.Height; y++)
                {
                    for (var x = xx; x < xx + this.cellSize && x < image.Width; x++)
                    {
                        bitmap.SetPixel(x, y, averageColor);
                    }
                }
            }
        }

        imagePart.StringBytes = Convert.ToBase64String(ImageToByteArray(bitmap));

        return imagePart;
    }
    
    private Image CreateImage(byte[] imageData)
    {
        Image image;
        using (var inStream = new MemoryStream())
        {
            inStream.Write(imageData, 0, imageData.Length);

            image = Bitmap.FromStream(inStream,true, false);
        }

        return image;
    }

    private byte[] ImageToByteArray(Image image)
    {
        byte[] temp = null;
        using (MemoryStream m = new MemoryStream())
        {
            image.Save(m, ImageFormat.Png);
            temp = m.ToArray();
        }

        return temp;
    } 
}