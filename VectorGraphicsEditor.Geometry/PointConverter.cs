using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geometry
{
    public class PointConverter : JsonConverter<ObservableCollection<Point>>
    {
        public override ObservableCollection<Point> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        {
            // Десериализуем как обычный List<Point>
            var points = JsonSerializer.Deserialize<List<Point>>(ref reader, options);
            return new ObservableCollection<Point>(points);
        }

        public override void Write(
            Utf8JsonWriter writer,
            ObservableCollection<Point> value,
            JsonSerializerOptions options)
        {
            // Сериализуем как обычный List<Point>
            JsonSerializer.Serialize(writer, value.ToList(), options);
        }
    }
}