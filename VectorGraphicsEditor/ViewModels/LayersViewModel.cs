using System;
using System.Linq.Expressions;
using System.Windows.Input;


using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GraphicsApp.ViewModels;

public partial class LayersViewModel : ViewModelBase
{
    [ObservableProperty]
    private MainWindowViewModel? _main;

    public ICommand SelectFigure { get; }
    public ICommand MoveUpFigure { get; }
    public ICommand MoveDownFigure { get; }

    public LayersViewModel(MainWindowViewModel? main)
    {
        Main = main;
        SelectFigure = new RelayCommand<ShapeViewModel>((figure) =>
        {
            if (Main is not null && figure is not null)
            {
                if (Main.SelectedFigure is not null)
                    Main.SelectedFigure.Active = false;
                Main.SelectedFigure = figure;
                figure.Active = true;
            }

        });
        MoveUpFigure = new RelayCommand<ShapeViewModel>(OnMoveUpButtonClick);
        MoveDownFigure = new RelayCommand<ShapeViewModel>(OnMoveDownButtonClick);
    }

    private void OnMoveUpButtonClick(ShapeViewModel figure)
    {
        var indexEl = Main.Figures.IndexOf(figure);
        if (indexEl > 0)
        {
            Main.History.Execute(new MoveLayer(
                Main,
                indexEl,
                indexEl - 1
                ));
        }
    }

    private void OnMoveDownButtonClick(ShapeViewModel figure)
    {
        var indexEl = Main.Figures.IndexOf(figure);
        if (indexEl < Main.Figures.Count - 1)
        {
            Main.History.Execute(new MoveLayer(
                Main,
                indexEl,
                indexEl + 1
                ));
        }
    }

    public class MoveLayer(MainWindowViewModel main, int index, int newIndex) : IUndoCommand
    {
        private readonly MainWindowViewModel _main = main;
        private readonly int _oldIndex = index;
        private readonly int _newIndex = newIndex;
        public void Execute()
        {
            (_main.Figures[_oldIndex], _main.Figures[_newIndex]) = (_main.Figures[_newIndex], _main.Figures[_oldIndex]);

            _main.Figures[_oldIndex].Active = false;
            _main.SelectedFigure = _main.Figures[_newIndex];
            _main.Figures[_newIndex].Active = true;
        }
        public void Undo()
        {
            (_main.Figures[_oldIndex], _main.Figures[_newIndex]) = (_main.Figures[_newIndex], _main.Figures[_oldIndex]);

            _main.Figures[_newIndex].Active = false;
            _main.SelectedFigure = _main.Figures[_oldIndex];
            _main.Figures[_oldIndex].Active = true;
        }
    }
}