using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Input;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Geometry;

using IO;
using IO.exporters;

using VecEditor.IO;


namespace GraphicsApp.ViewModels;

public enum Tools
{
    Cursor,
    Pen,
    PenAdd,
    PenRemove,
    Fill,
    Rotate,
    SelectFigure,
    None
}


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private FooterViewModel _footerview;

    [ObservableProperty]
    private CanvasViewModel _canvasview;

    [ObservableProperty]
    private ToolBarsViewModel _toolbarsview;

    [ObservableProperty]
    private LayersViewModel _layersview;


    [ObservableProperty]
    private SettingsWindowViewModel _settingswindow;

    [ObservableProperty]
    private SettingsViewModel _settings;

    [ObservableProperty]
    private Tools _selectTool = Tools.Cursor;

    public ObservableCollection<ShapeViewModel> Figures { get; } = [];


    public ObservableCollection<ModelFactoryViewModel> Factories { get; } = [];

    public CommandHistory History { get; set; } = new();

    [ObservableProperty]
    private ModelFactoryViewModel? _selectedButtonFigure;

    private ShapeViewModel? _selectedFigure;

    public ShapeViewModel? SelectedFigure
    {
        get => _selectedFigure;
        set
        {
            if (_selectedFigure is not null)
                _selectedFigure.Active = false;
            if (value is not null)
                value.Active = true;
            _selectedFigure = value;
            OnPropertyChanged(nameof(SelectedFigure));
        }
    }

    public ICommand SaveJsonCommand { get; }
    public ICommand LoadJsonCommand { get; }
    public ICommand SaveSvgCommand { get; }
    public ICommand SavePngCommand { get; }
    public ICommand ToDefaultSettingsAndClearCanvas { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }

    [ImportMany]
    private IEnumerable<ExportFactory<IShape, ModelMetadata>> ModelFactories { get; set; } = [];

    private readonly GeometryJsonSerializer<ShapeViewModel> _geometryJsonSerializer;
    public MainWindowViewModel()
    {
        var configuration = new ContainerConfiguration()
            .WithAssembly(typeof(IShape).Assembly);
        using var container = configuration.CreateContainer();
        container.SatisfyImports(this);

        foreach (var factory in ModelFactories)
        {
            Factories.Add(new() { Factory = factory, Main = this });
        }
        Footerview = new(this);
        Canvasview = new(this);
        Layersview = new(this);

        Toolbarsview = new(this);
        Settingswindow = new(this);
        Settings = new(this);
        _geometryJsonSerializer = new();

        SaveJsonCommand = new RelayCommand<string>(SaveFiguresJSON);

        LoadJsonCommand = new RelayCommand<string>(LoadFigures);

        SaveSvgCommand = new RelayCommand<string>(SaveToSVG);
        SavePngCommand = new RelayCommand<string>(SaveToPng);
        ToDefaultSettingsAndClearCanvas = new RelayCommand(ClearCanvas);
        UndoCommand = new RelayCommand(() => History.Undo());
        RedoCommand = new RelayCommand(() => History.Redo());

    }

    private void SaveFiguresJSON(string? filePath)
    {
        if (filePath is not null)
        {
            try
            {
                _geometryJsonSerializer.SaveJson(
                    filePath,
                    new Vector2(x: (float) Canvasview.OriginalWidth, y: (float) Canvasview.OriginalHeight),
                    Figures
                );
                if (!File.Exists(filePath)) Debug.WriteLine($"Warning: ���� {filePath} �� ��� ������!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }
    }

    private void LoadFigures(string? FilePath)
    {
        if (FilePath is null)
            return;

        try
        {
            var result = _geometryJsonSerializer.LoadJson(FilePath);

            if (result is null)
            {
                Debug.WriteLine($"Warning: Failed to read the selected file {FilePath}");
                return;
            }
            var CanvasSize = result.Value.CanvasSize;
            var figures = result.Value.Figures;

            if (figures is null)
                return;

            ToDefaultSettingsAndClearCanvas.Execute(null);
            Canvasview.ChangeSizeCanvas.Execute(CanvasSize);

            foreach (var fig in figures)
            {
                fig.Main = this;
                fig.Active = false;

                Figures.Add(fig);
            }
            History.Clear();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    private void SaveToSVG(string? filepath)
    {
        if (filepath is null) return;
        if (Figures.Count < 1) return;
        List<IShape> shapes = new List<IShape>();
        foreach (var fig in Figures)
        {
            shapes.Add(fig.Model);
        }
        try
        {
            SvgExporter.Export(filepath, new Vector2(x: (float) Canvasview.OriginalWidth, y: (float) Canvasview.OriginalHeight), shapes);
            if (!File.Exists(filepath)) Debug.WriteLine($"Warning: ���� {filepath} �� ��� ������!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    private void SaveToPng(string? filepath)
    {
        if (filepath is null) return;
        SelectedFigure = null;
        try
        {
            RasterExporter.Export(Canvasview.MainCanvas, filepath);
            if (!File.Exists(filepath)) Debug.WriteLine($"Warning: ���� {filepath} �� ��� ������!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    private void ClearCanvas()
    {
        Figures.Clear();
        SelectedFigure = null;
        SelectedButtonFigure = null;
        Canvasview.MainCanvas.Background = new SolidColorBrush(Colors.White);
        Canvasview.OriginalHeight = 500;
        Canvasview.OriginalWidth = 1000;
        Canvasview.ZoomFactor = 1;
        Toolbarsview.LineThickness = 0;
        Toolbarsview.OutlineColor = Colors.Black;
        Toolbarsview.SelectedColor = Colors.Red;
        History.Clear();
    }
}