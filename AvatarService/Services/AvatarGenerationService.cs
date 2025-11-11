using SkiaSharp;

namespace AvatarService.Services;

public class AvatarGenerationService 
{
    private readonly IAvatarStore _avatarStore;

    public AvatarGenerationService(IAvatarStore avatarStorageService) 
    {
        _avatarStore = avatarStorageService;
    }

    public string GenerateAvatar(string username) {
        var fileName = _avatarStore.NewAvatarPath("png");
        var firstChar = username[0];
        GenerateImage(firstChar.ToString(), _avatarStore.GetAvatarFileName(fileName));
        return fileName;
    }

    private static void GenerateImage(string text, string filePath, int width = 256, int height = 256)
    {
        // Create a new image surface
        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            
            // Clear the canvas with white background
            var randomColor = new Random();
            var color = SKColor.FromHsl((byte)randomColor.Next(360), (byte)randomColor.Next(100), 70);
            canvas.Clear(color);

            // canvas.Clear(SKColors.White);
            
            // Create paint for drawing text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 96,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Sans", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            };
            
            // Measure text to center it
            SKRect textBounds = new SKRect();
            paint.MeasureText(text, ref textBounds);
            
            // Calculate position to center the text
            float x = (width - textBounds.Width) / 2 - textBounds.Left;
            float y = (height - textBounds.Height) / 2 - textBounds.Top;
            
            // Draw the text
            canvas.DrawText(text, x, y, paint);
            
            // Save the image
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }
}
