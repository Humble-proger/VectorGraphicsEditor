using System.Numerics;
using System.Text.Json;

using Geometry;

namespace VecEditor.IO;

public class GeometryJsonSerializer<T>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = {
            new ColorConverter(),
            new PointConverter()
        },
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true
    };

    public void SaveJson(string filename, Vector2 canvasSize, IEnumerable<T> figures)
    {
        var data = new Dictionary<string, object>{
            { "CanvasSize", new { X = canvasSize.X, Y = canvasSize.Y } },
            { "Figures", figures }
        };

        File.WriteAllText(filename, JsonSerializer.Serialize(data, _jsonSerializerOptions));
    }

    public (Vector2 CanvasSize, IEnumerable<T>? Figures)? LoadJson(string filename)
    {
        if (!File.Exists(filename))
            return null;

        var json = File.ReadAllText(filename);
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var CanvasSizeElement = root.GetProperty("CanvasSize");
        var canvasSize = new Vector2(
            CanvasSizeElement.GetProperty("X").GetSingle(),
            CanvasSizeElement.GetProperty("Y").GetSingle()
            );

        var figuresElement = root.GetProperty("Figures");
        var figures = JsonSerializer.Deserialize<IEnumerable<T>>(figuresElement.GetRawText(), _jsonSerializerOptions);

        return (canvasSize, figures);
    }
}