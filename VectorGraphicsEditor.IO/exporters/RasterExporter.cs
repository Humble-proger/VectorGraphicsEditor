using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

using IO.interfaces;

namespace IO.exporters;

public static class RasterExporter
{
    public static void Export(Canvas canvas, string filepath)
    {
        if (canvas.Bounds.Width == 0 || canvas.Bounds.Height == 0)
        {
            throw new InvalidOperationException("The canvas does not have the correct dimensions. Make sure it is rendered.");
        }
        var pixelSize = new PixelSize((int) canvas.Bounds.Width, (int) canvas.Bounds.Height);
        var dpi = new Vector(96, 96);
        var renderBitmap = new RenderTargetBitmap(pixelSize, dpi);
        renderBitmap.Render(canvas);
        using (var stream = File.OpenWrite(filepath))
        {
            renderBitmap.Save(stream);
        }
    }
}