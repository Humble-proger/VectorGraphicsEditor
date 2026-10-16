using System;
using System.Collections.ObjectModel;

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using CommunityToolkit.Mvvm.ComponentModel;

namespace GraphicsApp.ViewModels
{
    public partial class ToolBarsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private MainWindowViewModel? _main;

        [ObservableProperty]
        private Color _selectedColor;

        [ObservableProperty]
        private Color _outlineColor; // Новое свойство для цвета контура

        [ObservableProperty]
        private double _lineThickness = 0.0;


        public ToolBarsViewModel(MainWindowViewModel? main)
        {
            Main = main;
            SelectedColor = Colors.Red;
            OutlineColor = Colors.Black;
        }
    }
}