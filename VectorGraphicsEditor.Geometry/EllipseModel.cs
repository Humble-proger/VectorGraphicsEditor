using System.Collections.ObjectModel;
using System.Composition;
using System.Text.Json.Serialization;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Geometry
{
    [Export(typeof(IShape))]
    [ExportMetadata("LogoButton", "ellipse.png")]
    [ExportMetadata("Name", "Ellipse")]
    public partial class EllipseModel : ObservableObject, IShape
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _strokeThickness = 1;

        [ObservableProperty]
        private Color _stroke = Colors.Black;

        [ObservableProperty]
        private Color _fill = Colors.Black;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Geometry))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _centerX;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Geometry))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _centerY;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Geometry))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _radiusX = 50;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Geometry))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _radiusY = 100;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Geometry))]
        [NotifyPropertyChangedFor(nameof(BoxWidth))]
        [NotifyPropertyChangedFor(nameof(BoxHeight))]
        [NotifyPropertyChangedFor(nameof(BoxCenterX))]
        [NotifyPropertyChangedFor(nameof(BoxCenterY))]
        private float _angle = 0;


        [JsonIgnore]
        public float BoxHeight => getBoxHeight() + StrokeThickness + 6;
        [JsonIgnore]
        public float BoxWidth => getBoxWidth() + StrokeThickness + 6;
        [JsonIgnore]
        public float BoxCenterX => CenterX - BoxWidth / 2;
        [JsonIgnore]
        public float BoxCenterY => CenterY - BoxHeight / 2;

        [JsonIgnore]
        public string Geometry => GetGeometry();

        [JsonIgnore]
        public float Width
        {
            get => 2 * RadiusX;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    RadiusX = value / 2;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
        [JsonIgnore]
        public float Height
        {
            get => 2 * RadiusY;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    RadiusY = value / 2;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }
        [JsonIgnore]
        public ObservableCollection<Point> ListOfPoints => [];

        public void Scale(float ratioX, float ratioY)
        {
            RadiusX *= float.Abs(ratioX);
            RadiusY *= float.Abs(ratioY);
            if (RadiusX <= 0.001)
                RadiusX = (float) 0.001;
            if (RadiusY <= 0.001)
                RadiusY = (float) 0.001;
        }

        public void Move(float deltaX, float deltaY)
        {
            CenterX += deltaX;
            CenterY += deltaY;
        }

        public void Rotate(float angle)
        {
            Angle = (Angle + angle) % 360;
        }

        public string GetGeometry()
        {
            var firstX = CenterX + RadiusX;
            var firstY = CenterY;
            var secondX = CenterX - RadiusX;
            var secondY = CenterY;

            var angleRad = Angle * Math.PI / 180.0;
            var firstPointX = (float) (CenterX + ((firstX - CenterX) * Math.Cos(angleRad) - (firstY - CenterY) * Math.Sin(angleRad)));
            var firstPointY = (float) (CenterY + ((firstX - CenterX) * Math.Sin(angleRad) + (firstY - CenterY) * Math.Cos(angleRad)));

            var secondPointX = (float) (CenterX + ((secondX - CenterX) * Math.Cos(angleRad) - (secondY - CenterY) * Math.Sin(angleRad)));
            var secondPointY = (float) (CenterY + ((secondX - CenterX) * Math.Sin(angleRad) + (secondY - CenterY) * Math.Cos(angleRad)));

            return FormattableString.Invariant($"M{firstPointX},{firstPointY} A{RadiusX},{RadiusY},{Angle},1,1,{secondPointX},{secondPointY} A{RadiusX},{RadiusY},{Angle},1,1,{firstPointX},{firstPointY} z");
        }

        private float getBoxWidth()
        {
            return (float) (2.0 * Math.Sqrt(Math.Pow(RadiusX * Math.Cos(Angle * Math.PI / 180.0), 2) + Math.Pow(RadiusY * Math.Sin(Angle * Math.PI / 180.0), 2)));
        }

        private float getBoxHeight()
        {
            return (float) (2.0 * Math.Sqrt(Math.Pow(RadiusX * Math.Sin(Angle * Math.PI / 180.0), 2) + Math.Pow(RadiusY * Math.Cos(Angle * Math.PI / 180.0), 2)));
        }
    }
}