using System.Numerics;
using System.Text;

using Avalonia.Controls.Shapes;
using Avalonia.Media;

using Geometry;

namespace IO
{
    public static class SvgExporter
    {
        private static string ConvertColorToHex(Color color)
        {
            if (color.A < 255)
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}"; // #RRGGBBAA
            else
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}"; // #RRGGBB
        }


        public static void Export(string filepath, Vector2 size, IEnumerable<IShape> figures)
        {
            if (size.X <= 0 || size.Y <= 0)
            {
                throw new InvalidOperationException("Canvas dimensions must be positive.");
            }


            var svgBuilder = new StringBuilder();

            svgBuilder.AppendLine($"<svg width=\"{size.X}\" height=\"{size.Y}\" xmlns=\"http://www.w3.org/2000/svg\">");

            foreach (var shape in figures)
            {
                string fillColor = ConvertColorToHex(shape.Fill);
                string strokeColor = ConvertColorToHex(shape.Stroke);
                float fillOpacity = shape.Fill.A / 255f; // Нормализация альфа-канала (0..1)
                float strokeOpacity = shape.Stroke.A / 255f;
                svgBuilder.AppendLine(
                    $@"<path 
                        d=""{shape.Geometry}""
                        fill=""{fillColor}""
                        {(shape.Fill.A < 255 ? $"fill-opacity=\"{fillOpacity:F2}\"" : "")}
                        stroke=""{strokeColor}""
                        {(shape.Stroke.A < 255 ? $"stroke-opacity=\"{strokeOpacity:F2}\"" : "")}
                        stroke-width=""{shape.StrokeThickness}""
                    />"
                );
            }

            svgBuilder.AppendLine("</svg>");

            File.WriteAllText(filepath, svgBuilder.ToString());
        }
    }
}