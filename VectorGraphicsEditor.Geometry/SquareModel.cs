using System.Collections.ObjectModel;
using System.Composition;
using System.Text.Json.Serialization;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Geometry
{
    [Export(typeof(IShape))]
    [ExportMetadata("LogoButton", "square.png")]
    [ExportMetadata("Name", "Square")]
    public partial class SquareModel : ObservableObject, IShape
    {
        private float _strokeThickness = 0;
        public float StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                var delta = value - _strokeThickness;
                var angleRad = _angle * Math.PI / 180.0;
                float cossin = float.Abs((float) Math.Cos(angleRad)) + float.Abs((float) Math.Sin(angleRad));
                BoxCenterX -= delta * cossin / 2;
                BoxCenterY -= delta * cossin / 2;
                BoxWidth += delta * cossin;
                BoxHeight += delta * cossin;
                _strokeThickness = value;
                OnPropertyChanged(nameof(StrokeThickness));
                OnPropertyChanged(nameof(Geometry));
            }
        }

        [ObservableProperty]
        [JsonConverter(typeof(ColorConverter))]
        private Color _stroke = Colors.Black;

        [ObservableProperty]
        [JsonConverter(typeof(ColorConverter))]
        private Color _fill = Colors.Black;

        private float _centerX = 0;
        public float CenterX
        {
            get => _centerX;
            set
            {
                var deltaX = value - _centerX;
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    var point = ListOfPoints[i];
                    point.X += deltaX;
                    ListOfPoints[i] = point;
                }
                _centerX = value;
                BoxCenterX += deltaX;
                OnPropertyChanged(nameof(CenterX));
                OnPropertyChanged(nameof(Geometry));
            }
        }
        private float _centerY = 0;
        public float CenterY
        {
            get => _centerY;
            set
            {
                var deltaY = value - _centerY;
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    var point = ListOfPoints[i];
                    point.Y += deltaY;
                    ListOfPoints[i] = point;
                }
                _centerY = value;
                BoxCenterY += deltaY;
                OnPropertyChanged(nameof(CenterY));
                OnPropertyChanged(nameof(Geometry));
            }
        }
        private float _width = 50;
        public float Width
        {
            get => _width;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    var ratioX = value / _width;
                    double minValX = double.MaxValue;
                    double minValY = double.MaxValue;

                    for (int i = 0; i < ListOfPoints.Count; i++)
                    {
                        var point = ListOfPoints[i];
                        point.X = CenterX + (point.X - CenterX) * ratioX;
                        point.Y = CenterY + (point.Y - CenterY) * float.Abs(ratioX);
                        ListOfPoints[i] = point;

                        if (point.X < minValX) minValX = point.X;
                        if (point.Y < minValY) minValY = point.Y;
                    }

                    _width = value;
                    var angleRad = _angle * Math.PI / 180.0;
                    float cossin = float.Abs((float) Math.Cos(angleRad)) + float.Abs((float) Math.Sin(angleRad));
                    BoxCenterX = (float) (minValX - StrokeThickness * cossin / 2 - 3);
                    BoxCenterY = (float) (minValY - StrokeThickness * cossin / 2 - 3);
                    BoxHeight = float.Abs(_width) + StrokeThickness * cossin + 6;
                    BoxWidth = float.Abs(_width) + StrokeThickness * cossin + 6;
                    OnPropertyChanged(nameof(Width));
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Geometry));
                }
            }
        }
        [JsonIgnore]
        public float Height
        {
            get => _width;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    var ratioX = value / _width;
                    double minValX = double.MaxValue;
                    double minValY = double.MaxValue;

                    for (int i = 0; i < ListOfPoints.Count; i++)
                    {
                        var point = ListOfPoints[i];
                        point.X = CenterX + (point.X - CenterX) * float.Abs(ratioX);
                        point.Y = CenterY + (point.Y - CenterY) * ratioX;
                        ListOfPoints[i] = point;

                        if (point.X < minValX) minValX = point.X;
                        if (point.Y < minValY) minValY = point.Y;
                    }

                    _width = value;
                    var angleRad = _angle * Math.PI / 180.0;
                    float cossin = float.Abs((float) Math.Cos(angleRad)) + float.Abs((float) Math.Sin(angleRad));
                    BoxCenterX = (float) (minValX - StrokeThickness * cossin / 2 - 3);
                    BoxCenterY = (float) (minValY - StrokeThickness * cossin / 2 - 3);
                    BoxHeight = float.Abs(_width) + StrokeThickness * cossin + 6;
                    BoxWidth = float.Abs(_width) + StrokeThickness * cossin + 6;
                    OnPropertyChanged(nameof(Width));
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Geometry));
                }
            }
        }
        private float _angle = 0;
        public float Angle
        {
            get => _angle;
            set
            {
                var angleRad = (value - _angle) * Math.PI / 180.0;
                float cos = (float) Math.Cos(angleRad);
                float sin = (float) Math.Sin(angleRad);

                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    var point = ListOfPoints[i];
                    float deltaX = (float) (point.X - CenterX);
                    float deltaY = (float) (point.Y - CenterY);

                    point.X = CenterX + deltaX * cos - deltaY * sin;
                    point.Y = CenterY + deltaX * sin + deltaY * cos;
                    ListOfPoints[i] = point;
                }
                _angle = value % 360;
                UpdateBox();
                OnPropertyChanged(nameof(Angle));
                OnPropertyChanged(nameof(Geometry));
            }
        }


        [ObservableProperty]
        private float _boxHeight;


        [ObservableProperty]
        private float _boxWidth;


        [ObservableProperty]
        private float _boxCenterX;


        [ObservableProperty]
        private float _boxCenterY;

        //[JsonIgnore]
        //private List<Point> ListOfPoints { get; set; }

        [JsonIgnore]
        public string Geometry => GetGeometry();

        [JsonConverter(typeof(PointConverter))]
        public ObservableCollection<Point> ListOfPoints { get; set; }

        [JsonIgnore]
        ObservableCollection<Point> IShape.ListOfPoints => [];

        public void Scale(float ratioX, float ratioY)
        {
            int invert = -1;
            if (ratioX == 1.0)
            {
                ratioX = ratioY;
                invert = 1;
            }
            else if (ratioY != 1.0)
                return;

            double minValX = double.MaxValue;
            double minValY = double.MaxValue;

            if (ratioX < 0)
            {
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    var point = ListOfPoints[i];
                    point.X = CenterX + (point.X - CenterX) * ratioX * invert;
                    point.Y = CenterY + (point.Y - CenterY) * -ratioX * invert;
                    ListOfPoints[i] = point;

                    if (point.X < minValX) minValX = point.X;
                    if (point.Y < minValY) minValY = point.Y;
                }
            }
            else
            {
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    var point = ListOfPoints[i];
                    point.X = CenterX + (point.X - CenterX) * ratioX;
                    point.Y = CenterY + (point.Y - CenterY) * ratioX;
                    ListOfPoints[i] = point;

                    if (point.X < minValX) minValX = point.X;
                    if (point.Y < minValY) minValY = point.Y;
                }
            }

            _width *= ratioX;
            var angleRad = _angle * Math.PI / 180.0;
            var cossin = float.Abs((float) Math.Cos(angleRad)) + float.Abs((float) Math.Sin(angleRad));

            BoxCenterX = (float) (minValX - StrokeThickness * cossin / 2 - 3);
            BoxCenterY = (float) (minValY - StrokeThickness * cossin / 2 - 3);
            BoxWidth = float.Abs(_width) + StrokeThickness * cossin + 6;
            BoxHeight = float.Abs(_width) + StrokeThickness * cossin + 6;
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(Geometry));
        }

        public void Move(float deltaX, float deltaY)
        {
            for (int i = 0; i < ListOfPoints.Count; i++)
            {
                var point = ListOfPoints[i];
                point.X += deltaX;
                point.Y += deltaY;
                ListOfPoints[i] = point;
            }
            _centerX += deltaX;
            _centerY += deltaY;
            BoxCenterX += deltaX;
            BoxCenterY += deltaY;
            OnPropertyChanged(nameof(CenterX));
            OnPropertyChanged(nameof(CenterY));
            OnPropertyChanged(nameof(Geometry));
        }

        public void Rotate(float angle)
        {
            Angle += angle;
        }

        [JsonConstructor]
        public SquareModel(float angle, float centerX, float centerY, float strokeThickness, Color fill, Color stroke, float width, float boxWidth, float boxHeight, float boxCenterX, float boxCenterY, ObservableCollection<Point> listOfPoints)
        {
            _angle = angle;
            _centerX = centerX;
            _centerY = centerY;
            _strokeThickness = strokeThickness;
            _fill = fill;
            _stroke = stroke;
            _width = width;
            BoxCenterX = boxCenterX;
            BoxCenterY = boxCenterY;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
            ListOfPoints = listOfPoints;

        }

        public SquareModel()
        {
            ListOfPoints = new ObservableCollection<Point>() {
                new Point(CenterX - Width / 2, CenterY - Width / 2),
                new Point(CenterX + Width / 2, CenterY - Width / 2),
                new Point(CenterX + Width / 2, CenterY + Width / 2),
                new Point(CenterX - Width / 2, CenterY + Width / 2)
            };
            UpdateBox();
        }

        private string GetGeometry()
        {
            var a = "";
            if (ListOfPoints.Count > 0)
            {
                a = FormattableString.Invariant($"M{ListOfPoints[0].X},{ListOfPoints[0].Y} ");
            }

            for (int i = 1; i < ListOfPoints.Count; i++)
            {
                a += FormattableString.Invariant($"L{ListOfPoints[i].X},{ListOfPoints[i].Y} ");
            }

            a += "z";
            return a;
        }
        private void UpdateBox()
        {
            Point minval, maxval;
            (minval, maxval) = FindMinMax();

            _width = (float) (maxval.X - minval.X);
            var anglerad = _angle * Math.PI / 180.0;
            var cossin = float.Abs((float) Math.Cos(anglerad)) + float.Abs((float) Math.Sin(anglerad));
            BoxCenterX = (float) minval.X - StrokeThickness * cossin / 2 - 3;
            BoxCenterY = (float) minval.Y - StrokeThickness * cossin / 2 - 3;
            BoxWidth = float.Abs(_width) + StrokeThickness * cossin + 6;
            BoxHeight = float.Abs(_width) + StrokeThickness * cossin + 6;
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
        }

        private (Point minPoints, Point maxPoints) FindMinMax()
        {
            double minValue1 = double.MaxValue;
            double minValue2 = double.MaxValue;
            double maxValue1 = double.MinValue;
            double maxValue2 = double.MinValue;

            foreach (var point in ListOfPoints)
            {
                if (point.X < minValue1) minValue1 = point.X;
                if (point.X > maxValue1) maxValue1 = point.X;
                if (point.Y < minValue2) minValue2 = point.Y;
                if (point.Y > maxValue2) maxValue2 = point.Y;
            }

            return (new Point(minValue1, minValue2), new Point(maxValue1, maxValue2));
        }
    }

}