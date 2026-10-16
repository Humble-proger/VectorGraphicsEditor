using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace IO;

public static class CanvasFactory
{
    public static Canvas CreateTestCanvas()
    {
        var canvas = new Canvas
        {
            Width = 500,
            Height = 500,
            Background = Brushes.Gray,
        };

        var rect = new Rectangle
        {
            Width = 200,
            Height = 50,
            Fill = Brushes.Brown,
            StrokeThickness = 2
        };
        Canvas.SetLeft(rect, 50);
        Canvas.SetTop(rect, 50);
        canvas.Children.Add(rect);

        // Добавляем эллипс
        var ellipse = new Ellipse
        {
            Width = 80,
            Height = 80,
            Fill = Brushes.Blue,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        Canvas.SetLeft(ellipse, 200);
        Canvas.SetTop(ellipse, 100);
        canvas.Children.Add(ellipse);

        canvas.Measure(new Size(canvas.Width, canvas.Height));
        canvas.Arrange(new Rect(0, 0, canvas.Width, canvas.Height));

        return canvas;
    }
}