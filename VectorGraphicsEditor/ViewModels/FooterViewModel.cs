using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GraphicsApp.ViewModels
{
    public partial class FooterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private MainWindowViewModel? _main;

        [ObservableProperty]
        private string _mouseCoords;

        [ObservableProperty]
        private string _canvasSize;

        [ObservableProperty]
        private string _zoomString = "100%";

        public ICommand ZoomIn { get; }
        public ICommand ZoomOut { get; }

        public FooterViewModel(MainWindowViewModel? main)
        {
            Main = main;
            MouseCoords = "";
            CanvasSize = $"1000 x 500";
            ZoomIn = new RelayCommand(() => Main.History.Execute(new ZoomCanvas(Main, 0.1)));
            ZoomOut = new RelayCommand(() => Main.History.Execute(new ZoomCanvas(Main, -0.1)));
        }
    }

    public class ZoomCanvas(MainWindowViewModel main, double delta) : IUndoCommand
    {
        private readonly MainWindowViewModel _main = main;
        private readonly double _delta = delta;
        public void Execute()
        {
            _main.Canvasview.Zoom(_delta);
        }
        public void Undo()
        {
            _main.Canvasview.Zoom(-_delta);
        }
    }
}