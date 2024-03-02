using SkiaSharp;

namespace DrawingViewPerf;

public class ImageUtils
{
    public static byte[] CreateImagePng(Stream imageStream)
    {
        // Decode the PNG images from memory streams
        SKBitmap bitmap1 = SKBitmap.Decode(imageStream);

        //Encode the merged bitmap to a PNG format
        byte[] imageBytes;
        using (SKImage image = SKImage.FromBitmap(bitmap1))
        {
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                imageBytes = data.ToArray();
            }
        }
        return imageBytes;
    }

    public static byte[] MergeImages(Stream imageStream1, Stream imageStream2)
    {
        // Decode the PNG images from memory streams
        SKBitmap bitmap1 = SKBitmap.Decode(imageStream1);
        SKBitmap bitmap2 = SKBitmap.Decode(imageStream2);

        // Calculate the maximum width and height of the merged image
        int maxWidth = Math.Max(bitmap1.Width, bitmap2.Width);
        int maxHeight = Math.Max(bitmap1.Height, bitmap2.Height);

        // Create a new bitmap to hold the merged image
        SKBitmap mergedBitmap = new SKBitmap(maxWidth, maxHeight);

        // Create a canvas from the merged bitmap
        using (SKCanvas canvas = new SKCanvas(mergedBitmap))
        {
            // Draw the first image on the canvas
            canvas.DrawBitmap(bitmap1, 0, 0);
            // Draw the second image on top of the first image with alpha blending
            using (var paint = new SKPaint { BlendMode = SKBlendMode.SrcOver })
            {
                canvas.DrawBitmap(bitmap2, SKRect.Create(maxWidth, maxHeight), paint);
            }
        }

        //Encode the merged bitmap to a PNG format
        byte[] mergedBytes;
        using (SKImage image = SKImage.FromBitmap(mergedBitmap))
        {
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                mergedBytes = data.ToArray();
            }
        }
        return mergedBytes;
    }
}
