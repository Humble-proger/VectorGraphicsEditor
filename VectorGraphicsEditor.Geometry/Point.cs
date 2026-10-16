
namespace Geometry
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x; Y = y;
        }

        public void Deconstruct(out double _startpositionX, out double _startpositionY)
        {
            _startpositionX = X; _startpositionY = Y;
        }
    }
}