using System.Collections.ObjectModel;
using System.Composition;
using System.Text.Json.Serialization;

using Avalonia;
using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Geometry
{
    [Export(typeof(IShape))]
    [ExportMetadata("LogoButton", "circle.png")]
    [ExportMetadata("Name", "Circle")]
    public partial class CircleModel : ObservableObject, IShape
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
        private float _radius = 50;

        [ObservableProperty]
        private float _angle = 0;

        [JsonIgnore]
        public float BoxHeight => float.Abs(2 * Radius) + StrokeThickness + 6;
        [JsonIgnore]
        public float BoxWidth => float.Abs(2 * Radius) + StrokeThickness + 6;
        [JsonIgnore]
        public float BoxCenterX => CenterX - BoxWidth / 2;
        [JsonIgnore]
        public float BoxCenterY => CenterY - BoxHeight / 2;
        [JsonIgnore]
        public string Geometry => getGeometry();

        [JsonIgnore]
        public float Width
        {
            get => 2 * Radius;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    Radius = value / 2;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
        [JsonIgnore]
        public float Height
        {
            get => 2 * Radius;
            set
            {
                if (float.Abs(value) > float.Epsilon)
                {
                    Radius = value / 2;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        [JsonIgnore]
        public ObservableCollection<Point> ListOfPoints => [];

        public void Scale(float ratioX, float ratioY)
        {
            if (ratioX == 1.0)
                Radius *= ratioY;
            else if (ratioY == 1.0)
                Radius *= ratioX;
            else
            {
                ratioX = ratioX >= ratioY ? ratioX : ratioY;
                Radius *= ratioX;
            }
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

        public string getGeometry()
        {
            var firstX = CenterX + Radius;
            var firstY = CenterY;
            var secondX = CenterX - Radius;
            var secondY = CenterY;

            var angleRad = Angle * Math.PI / 180.0;
            var firstPointX = (float) (CenterX + (firstX - CenterX) * Math.Cos(angleRad) - (firstY - CenterY) * Math.Sin(angleRad));
            var firstPointY = (float) (CenterY + (firstX - CenterX) * Math.Sin(angleRad) + (firstY - CenterY) * Math.Cos(angleRad));

            var secondPointX = (float) (CenterX + (secondX - CenterX) * Math.Cos(angleRad) - (secondY - CenterY) * Math.Sin(angleRad));
            var secondPointY = (float) (CenterY + (secondX - CenterX) * Math.Sin(angleRad) + (secondY - CenterY) * Math.Cos(angleRad));

            return FormattableString.Invariant($"M{firstPointX},{firstPointY} A{Radius},{Radius},{Angle},1,1,{secondPointX},{secondPointY} A{Radius},{Radius},{Angle},1,1,{firstPointX},{firstPointY} z");
        }
    }
}