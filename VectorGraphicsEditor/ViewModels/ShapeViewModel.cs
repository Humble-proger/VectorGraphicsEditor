using System.Text.Json.Serialization;
using System.Windows.Input;

using Avalonia.Animation;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Geometry;

using GraphicsApp.ViewModels;

using Tmds.DBus.Protocol;

namespace GraphicsApp.ViewModels
{
    public partial class ShapeViewModel : ViewModelBase
    {
        [ObservableProperty]
        [property: JsonIgnore]
        private MainWindowViewModel? _main;

        [JsonPropertyName("Name")]
        public string _name { get; set; } = "Name";

        [JsonIgnore]
        public string Name
        {
            get => _name;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is NameChengedCommand cv && cv.Shape == this)
                    {
                        cv.NewName = value;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new NameChengedCommand(this, value, _name));
                    }
                }
                else
                {
                    Main.History.Add(new NameChengedCommand(this, value, _name));
                }
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        [JsonPropertyName("IsVisibleIcon")]
        public bool _isVisibleIcon { get; set; } = true;

        [JsonIgnore]
        public bool IsVisibleIcon
        {
            get => _isVisibleIcon;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is not NameChengedCommand)
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ChengeVisibleFigure(this, _isVisibleIcon));
                    }
                }
                else
                {
                    Main.History.Add(new ChengeVisibleFigure(this, _isVisibleIcon));
                }
                _isVisibleIcon = value;
                OnPropertyChanged(nameof(IsVisibleIcon));
            }
        }

        public required IShape Model { get; init; }

        [JsonIgnore]
        public float CenterX
        {
            get => Model.CenterX;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is MoveFigure cv && cv._shape == this)
                    {
                        cv.DeltaX += value - Model.CenterX;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new MoveFigure(this, value - Model.CenterX, 0));
                    }
                }
                else
                {
                    Main.History.Add(new MoveFigure(this, value - Model.CenterX, 0));
                }
                Model.CenterX = value;
                OnPropertyChanged(nameof(CenterX));
            }
        }
        [JsonIgnore]
        public float CenterY
        {
            get => Model.CenterY;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is MoveFigure cv && cv._shape == this)
                    {
                        cv.DeltaY += value - Model.CenterY;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new MoveFigure(this, 0, value - Model.CenterY));
                    }
                }
                else
                {
                    Main.History.Add(new MoveFigure(this, 0, value - Model.CenterY));
                }
                Model.CenterY = value;
                OnPropertyChanged(nameof(CenterY));
            }
        }
        [JsonIgnore]
        public float Angle
        {
            get => Model.Angle;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is RotateFigure cv && cv._shape == this)
                    {
                        cv.DeltaTheta += value - Model.Angle;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new RotateFigure(this, value - Model.Angle));
                    }
                }
                else
                {
                    Main.History.Add(new RotateFigure(this, value - Model.Angle));
                }
                Model.Angle = value;
                OnPropertyChanged(nameof(Angle));
            }
        }
        [JsonIgnore]
        public float Width
        {
            get => Model.Width;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is ScaleXFigure cv && cv._shape == this)
                    {
                        cv.DeltaScaleX *= value / Model.Width;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ScaleXFigure(this, value / Model.Width));
                    }
                }
                else
                {
                    Main.History.Add(new ScaleXFigure(this, value / Model.Width));
                }
                Model.Width = value;
                UpdateSize();
            }
        }
        [JsonIgnore]
        public float Height
        {
            get => Model.Height;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is ScaleYFigure cv && cv._shape == this)
                    {
                        cv.DeltaScaleY *= value / Model.Height;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ScaleYFigure(this, value / Model.Height));
                    }
                }
                else
                {
                    Main.History.Add(new ScaleYFigure(this, value / Model.Height));
                }
                Model.Height = value;
                UpdateSize();
            }
        }
        [JsonIgnore]
        public Color Fill
        {
            get => Model.Fill;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is ChengeFillFigure cv && cv._shape == this)
                    {
                        cv.Color = value;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ChengeFillFigure(this, value));
                    }
                }
                else
                {
                    Main.History.Add(new ChengeFillFigure(this, value));
                }
                Model.Fill = value;
                OnPropertyChanged(nameof(Fill));
            }
        }

        [JsonIgnore]
        public Color Stroke
        {
            get => Model.Stroke;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is ChengeStrokeFigure cv && cv._shape == this)
                    {
                        cv.Color = value;
                        Main.History.Add(cv);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ChengeStrokeFigure(this, value));
                    }
                }
                else
                {
                    Main.History.Add(new ChengeStrokeFigure(this, value));
                }
                Model.Stroke = value;
                OnPropertyChanged(nameof(Stroke));
            }
        }

        [JsonIgnore]
        public float StrokeThickness
        {
            get => Model.StrokeThickness;
            set
            {
                if (Main.History.CanUndo)
                {
                    var temp = Main.History.Pop();
                    if (temp is ChengeStrokeThicknessFigure ch && ch.Shape == this)
                    {
                        ch.Delta += value - Model.StrokeThickness;
                        Main.History.Add(ch);
                    }
                    else
                    {
                        Main.History.Push(temp);
                        Main.History.Add(new ChengeStrokeThicknessFigure(this, value - Model.StrokeThickness));
                    }
                }
                else
                {
                    Main.History.Add(new ChengeStrokeThicknessFigure(this, value - Model.StrokeThickness));
                }
                Model.StrokeThickness = value;
                OnPropertyChanged(nameof(StrokeThickness));
            }
        }


        [ObservableProperty]
        [property: JsonIgnore]
        private bool _active = false;

        [JsonIgnore]
        public ICommand Remove { get; }
        public ShapeViewModel()
        {
            Remove = new RelayCommand(() => { Main.History.Execute(new DeleteFigure(this, Main)); });
        }
        public void UpdateCenterX()
        {
            OnPropertyChanged(nameof(CenterX));
        }
        public void UpdateCenterY()
        {
            OnPropertyChanged(nameof(CenterY));
        }
        public void UpdateSize()
        {
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
        }
        public void UpdateFill()
        {
            OnPropertyChanged(nameof(Fill));
        }
        public void UpdateStroke()
        {
            OnPropertyChanged(nameof(Stroke));
        }
        public void UpdateStrokeThickness()
        {
            OnPropertyChanged(nameof(StrokeThickness));
        }
        public void UpdateAngle()
        {
            OnPropertyChanged(nameof(Angle));
        }
        public void UpdateName()
        {
            OnPropertyChanged(nameof(Name));
        }
        public void UpdateIsVisibleIcon()
        {
            OnPropertyChanged(nameof(IsVisibleIcon));
        }
    }
}

public class DeleteFigure(ShapeViewModel figure, MainWindowViewModel main) : IUndoCommand
{

    public readonly ShapeViewModel _shape = figure;
    public readonly MainWindowViewModel _mainWindow = main;
    public int CurrentIndex { get; set; } = main.Figures.IndexOf(figure);
    public void Execute()
    {
        _mainWindow.Figures.Remove(_shape);
    }
    public void Undo()
    {
        _mainWindow.Figures.Insert(CurrentIndex, _shape);
    }
}

public class MoveFigure(ShapeViewModel figure, float deltaX, float deltaY) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public float DeltaX { get; set; } = deltaX;
    public float DeltaY { get; set; } = deltaY;
    public void Execute()
    {
        _shape.Model.Move(DeltaX, DeltaY);
        _shape.UpdateCenterX();
        _shape.UpdateCenterY();
    }
    public void Undo()
    {
        _shape.Model.Move(-DeltaX, -DeltaY);
        _shape.UpdateCenterX();
        _shape.UpdateCenterY();
    }
}

public class RotateFigure(ShapeViewModel figure, float deltaTheta) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public float DeltaTheta { get; set; } = deltaTheta;
    public void Execute()
    {
        _shape.Model.Rotate(DeltaTheta);
        _shape.UpdateAngle();
    }
    public void Undo()
    {
        _shape.Model.Rotate(-DeltaTheta);
        _shape.UpdateAngle();
    }
}

public class ScaleXFigure(ShapeViewModel figure, float deltaScaleX) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public float DeltaScaleX { get; set; } = deltaScaleX;
    public void Execute()
    {
        _shape.Model.Scale(DeltaScaleX, 1);
        _shape.UpdateSize();
    }
    public void Undo()
    {
        _shape.Model.Scale(1 / DeltaScaleX, 1);
        _shape.UpdateSize();
    }
}

public class ScaleYFigure(ShapeViewModel figure, float deltaScaleY) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public float DeltaScaleY { get; set; } = deltaScaleY;
    public void Execute()
    {
        _shape.Model.Scale(1, DeltaScaleY);
        _shape.UpdateSize();
    }
    public void Undo()
    {
        _shape.Model.Scale(1, 1 / DeltaScaleY);
        _shape.UpdateSize();
    }
}

public class NameChengedCommand(ShapeViewModel figure, string _newName, string _oldName) : IUndoCommand
{
    public string OldName { get; set; } = _oldName;
    public string NewName { get; set; } = _newName;
    public readonly ShapeViewModel Shape = figure;
    public void Execute()
    {
        Shape._name = NewName;
        Shape.UpdateName();
    }
    public void Undo()
    {
        Shape._name = OldName;
        Shape.UpdateName();
    }
}

public class ChengeVisibleFigure(ShapeViewModel figure, bool _oldValue) : IUndoCommand
{
    public bool OldValue { get; set; } = _oldValue;
    public readonly ShapeViewModel Shape = figure;
    public void Execute()
    {
        Shape._isVisibleIcon = !OldValue;
        Shape.UpdateIsVisibleIcon();
    }
    public void Undo()
    {
        Shape._isVisibleIcon = OldValue;
        Shape.UpdateIsVisibleIcon();
    }
}

public class MovePointFigure(ShapeViewModel figure, float deltaX, float deltaY, int index) : IUndoCommand
{
    public float DeltaX { get; set; } = deltaX;
    public float DeltaY { get; set; } = deltaY;
    public int Index { get; set; } = index;
    public readonly ShapeViewModel _typefigure = figure;
    public void Execute()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("MovePoint") != null)
        {
            typefigure.MovePoint(DeltaX, DeltaY, Index);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();
        }
    }
    public void Undo()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("MovePoint") != null)
        {
            typefigure.MovePoint(-DeltaX, -DeltaY, Index);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();
        }
    }
}

public class AddPointFigure(ShapeViewModel figure, float posX, float posY) : IUndoCommand
{
    public readonly ShapeViewModel _typefigure = figure;
    public Point Point { get; set; } = new Point(posX, posY);
    public void Execute()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("AddPoint") != null)
        {
            typefigure.AddPoint(Point);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();

        }
    }
    public void Undo()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("RemovePoint") != null)
        {
            typefigure.RemovePoint(Point);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();
        }
    }
}

public class RemovePointFigure(ShapeViewModel figure, Point point) : IUndoCommand
{
    public readonly ShapeViewModel _typefigure = figure;
    public Point Point { get; set; } = point;
    public void Execute()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("RemovePoint") != null)
        {
            typefigure.RemovePoint(Point);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();
        }
    }
    public void Undo()
    {
        dynamic typefigure = _typefigure.Model;
        if (typefigure.GetType().GetMethod("AddPoint") != null)
        {
            typefigure.AddPoint(Point);
            _typefigure.UpdateCenterX();
            _typefigure.UpdateCenterY();
            _typefigure.UpdateSize();
        }
    }
}

public class ChengeFillFigure(ShapeViewModel figure, Color color) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public Color Color { get; set; } = color;
    public Color Predcolor { get; set; } = figure.Model.Fill;

    public void Execute()
    {
        _shape.Model.Fill = Color;
        _shape.UpdateFill();
    }
    public void Undo()
    {
        _shape.Model.Fill = Predcolor;
        _shape.UpdateFill();
    }
}

public class ChengeStrokeFigure(ShapeViewModel figure, Color color) : IUndoCommand
{
    public readonly ShapeViewModel _shape = figure;
    public Color Color { get; set; } = color;
    public Color Predcolor { get; set; } = figure.Model.Stroke;

    public void Execute()
    {
        _shape.Model.Stroke = Color;
        _shape.UpdateStroke();
    }
    public void Undo()
    {
        _shape.Model.Stroke = Predcolor;
        _shape.UpdateStroke();
    }
}

public class ChengeStrokeThicknessFigure(ShapeViewModel figure, float delta) : IUndoCommand
{
    public readonly ShapeViewModel Shape = figure;
    public float Delta { get; set; } = delta;

    public void Execute()
    {
        Shape.Model.StrokeThickness += Delta;
        Shape.UpdateStrokeThickness();
    }
    public void Undo()
    {
        Shape.Model.StrokeThickness -= Delta;
        Shape.UpdateStrokeThickness();
    }
}