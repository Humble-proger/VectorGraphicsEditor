using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

using Avalonia.Media;

namespace Geometry
{

    // интерфейс всех фигур
    [JsonDerivedType(typeof(CircleModel), typeDiscriminator: "circle")]
    [JsonDerivedType(typeof(RectangleModel), typeDiscriminator: "rectangle")]
    [JsonDerivedType(typeof(SquareModel), typeDiscriminator: "square")]
    [JsonDerivedType(typeof(EllipseModel), typeDiscriminator: "ellipse")]
    [JsonDerivedType(typeof(TriangleModel), typeDiscriminator: "triangle")]
    [JsonDerivedType(typeof(HexagonModel), typeDiscriminator: "hexagon")]
    [JsonDerivedType(typeof(BezierCurveModel), typeDiscriminator: "beziercurve")]
    [JsonDerivedType(typeof(PolygonModel), typeDiscriminator: "polygon")]
    [JsonDerivedType(typeof(RhombModel), typeDiscriminator: "rhomb")]
    public interface IShape
    {
        // ширина описанного прямоугольника
        float BoxWidth { get; }
        // высота описанного прямоугольника
        float BoxHeight { get; }
        // координаты центра описанного прямоугольника
        float BoxCenterX { get; }
        float BoxCenterY { get; }
        // координата х центра фигуры
        float CenterX { get; set; }
        // координата y центра фигуры
        float CenterY { get; set; }

        float Width { get; set; }
        float Height { get; set; }
        float Angle { get; set; }
        // толщина обводки)
        float StrokeThickness { get; set; }
        // цвет обводки
        Color Stroke { get; set; }
        // цвет заливки
        Color Fill { get; set; }
        ObservableCollection<Point> ListOfPoints { get; }
        public void Move(float deltaX, float deltaY);
        public void Scale(float ratioX, float ratioY);
        public void Rotate(float angle);
        public string Geometry { get; }
    }
}