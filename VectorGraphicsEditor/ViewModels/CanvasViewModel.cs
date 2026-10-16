using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GraphicsApp.ViewModels
{

    public partial class CanvasViewModel : ViewModelBase
    {
        private double _originalWidth = 1000;

        private double _originalHeight = 500;

        [ObservableProperty]
        private Canvas _mainCanvas;

        public double OriginalWidth
        {
            get => _originalWidth;
            set
            {
                _originalWidth = value;
                OnPropertyChanged(nameof(OriginalWidth));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

        public double OriginalHeight
        {
            get => _originalHeight;
            set
            {
                _originalHeight = value;
                OnPropertyChanged(nameof(OriginalHeight));
                OnPropertyChanged(nameof(ScaledHeight));
            }
        }

        [ObservableProperty]
        private MainWindowViewModel? _main;

        public ICommand ChangeMouseCoord { get; }
        public ICommand MouseLeaveCanvas { get; }
        public ICommand ChangeSizeCanvas { get; }

        public double ScaledWidth => OriginalWidth * ZoomFactor;

        public double ScaledHeight => OriginalHeight * ZoomFactor;


        public CanvasViewModel(MainWindowViewModel? main)
        {
            Main = main;
            ChangeMouseCoord = new RelayCommand<Avalonia.Point>(OnMouseMove);
            MouseLeaveCanvas = new RelayCommand(OnMouseLeave);
            ChangeSizeCanvas = new RelayCommand<Vector2>(OnResizeCanvas);
        }

        private void OnMouseMove(Avalonia.Point Point)
        {
            if (Main is not null)
                Main.Footerview.MouseCoords = $"X: {Point.X,5:F1}, Y {Point.Y,5:F1}";
        }

        private void OnMouseLeave()
        {
            if (Main is not null)
                Main.Footerview.MouseCoords = "";
        }

        private void OnResizeCanvas(Vector2 size)
        {
            Main.History.Execute(new ChengeSizeCanvasCommand(Main, size.X, size.Y));
        }
        private double _zoomFactor = 1.0;

        public double ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                SetProperty(ref _zoomFactor, value);
                if (Main is not null)
                    Main.Footerview.ZoomString = $"{Math.Round(value * 100)}%";
                OnPropertyChanged(nameof(ScaledHeight));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

        public void Zoom(double delta)
        {
            var temp = ZoomFactor + delta;
            if (temp < 5 && temp > 0.1)
            {
                ZoomFactor = temp;
            }
        }
    }
    public class OffsetConverter : IValueConverter
    {
        public static OffsetConverter Instance { get; } = new OffsetConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double coord && parameter is string offsetStr && double.TryParse(offsetStr, out double offset))
            {
                return coord - offset;
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ChengeSizeCanvasCommand(MainWindowViewModel main, double newWidth, double newHeight) : IUndoCommand
    {
        private readonly MainWindowViewModel _main = main;
        private readonly double _oldWidth = main.Canvasview.OriginalWidth;
        private readonly double _oldHeight = main.Canvasview.OriginalHeight;
        private readonly double _newWidth = newWidth;
        private readonly double _newHeight = newHeight;
        public void Execute()
        {
            _main.Footerview.CanvasSize = $"{_newWidth,5:F1} x {_newHeight,5:F1}";
            _main.Canvasview.OriginalWidth = _newWidth;
            _main.Canvasview.OriginalHeight = _newHeight;
        }
        public void Undo()
        {
            _main.Footerview.CanvasSize = $"{_oldWidth,5:F1} x {_oldHeight,5:F1}";
            _main.Canvasview.OriginalWidth = _oldWidth;
            _main.Canvasview.OriginalHeight = _oldHeight;
        }
    }
}