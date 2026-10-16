using System.Text.Json;
using System.Text.Json.Serialization;

using Avalonia.Media;

namespace Geometry
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Чтение JSON-объекта
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                byte a = root.GetProperty("A").GetByte();
                byte r = root.GetProperty("R").GetByte();
                byte g = root.GetProperty("G").GetByte();
                byte b = root.GetProperty("B").GetByte();
                return Color.FromArgb(a, r, g, b);
            }
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            // Запись JSON-объекта
            writer.WriteStartObject();
            writer.WriteNumber("A", value.A);
            writer.WriteNumber("R", value.R);
            writer.WriteNumber("G", value.G);
            writer.WriteNumber("B", value.B);
            writer.WriteEndObject();
        }
    }
}