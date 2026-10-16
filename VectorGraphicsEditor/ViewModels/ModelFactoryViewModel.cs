using System;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Numerics;
using System.Windows.Input;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Geometry;

using GraphicsApp.ViewModels;

namespace GraphicsApp.ViewModels
{
    public partial class ModelFactoryViewModel : ViewModelBase
    {
        [ObservableProperty]
        private MainWindowViewModel? _main;
        public required ExportFactory<IShape, ModelMetadata> Factory { get; init; }
        public ICommand CreateCommand { get; }

        public ModelFactoryViewModel()
        {
            CreateCommand = new RelayCommand<Avalonia.Point>((Avalonia.Point point) => Create(point));
        }

        private void Create(Avalonia.Point point)
        {
            var colorFill = Main.Toolbarsview.SelectedColor;
            var tickness = Main.Toolbarsview.LineThickness;
            var colorBorder = Main.Toolbarsview.OutlineColor;

            var model = Factory.CreateExport().Value;
            var countFigures = Main.Figures.Count(elem => elem.Name.Split()[0] == Factory.Metadata.Name);
            var name = $"{Factory.Metadata.Name} {countFigures + 1}";
            model.Fill = colorFill;
            model.Stroke = colorBorder;
            model.StrokeThickness = (float) tickness;
            model.Move((float) point.X, (float) point.Y);
            model.StrokeThickness = (float) Main.Toolbarsview.LineThickness;

            var figure = new ShapeViewModel() { _name = name, Model = model, Main = Main };
            Main.History.Execute(new CreateFigure(
                figure,
                Main
                ));
        }
    }

    public class CreateFigure(ShapeViewModel figure, MainWindowViewModel main) : IUndoCommand
    {

        private readonly ShapeViewModel _shape = figure;
        private readonly MainWindowViewModel _mainWindow = main;
        public void Execute()
        {
            _mainWindow.Figures.Add(_shape);
        }
        public void Undo()
        {
            _mainWindow.Figures.Remove(_shape);
        }
    }

}