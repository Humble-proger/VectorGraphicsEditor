using System;
using System.Numerics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

using Geometry;

using GraphicsApp.ViewModels;

using Point = Geometry.Point;

namespace GraphicsApp.Views
{
    public partial class MyCanvas : UserControl
    {
        private enum Operation
        {
            Translate,
            Rotate,
            ScaleX,
            ScaleY,
            None
        }

        private bool _isReflectionX = false;
        private bool _isReflectionY = false;
        private double deltaX = 0;
        private double deltaY = 0;
        private double deltaTheta = 0;
        private float deltaScaleX = 1;
        private float deltaScaleY = 1;
        private double _startAngle = 0.0;
        private double _startpositionX;
        private double _startpositionY;
        private int _inversionScale = 1;
        private double _oldTheta = 0.0;
        private int _indexPoint = -1;

        private Operation _isFigureResize = Operation.None;
        public MyCanvas()
        {
            InitializeComponent();
        }

        private double CalcAngle(double deltaX, double deltaY)
        {
            double theta;
            if (double.Abs(deltaX) < double.Epsilon)
                if (deltaY >= 0)
                    theta = 90;
                else
                    theta = -90;
            else
            {
                theta = Math.Atan(deltaY / deltaX) * (180 / Math.PI);
                if (deltaX < 0)
                    theta += 180;
            }
            return theta;
        }

        private void MouseMoved(object sender, PointerEventArgs args)
        {

            if (DataContext is CanvasViewModel viewModel)
            {

                var point = args.GetPosition((Canvas) sender);
                viewModel.ChangeMouseCoord.Execute(point);
            }
        }

        private void MouseLeave(object sender, RoutedEventArgs e)
        {
            if (DataContext is CanvasViewModel viewModel)
                viewModel.MouseLeaveCanvas.Execute(null);
        }

        private void CanvasResize(object sender, SizeChangedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                canvas.Clip = new RectangleGeometry(new Rect(0, 0, e.NewSize.Width, e.NewSize.Height));
            }
        }

        private void CreateFigure(object sender, PointerPressedEventArgs e)
        {
            if (e.Handled)
                return;
            if (DataContext is CanvasViewModel viewModel && sender is Canvas _canvas && e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
            {

                if (viewModel.Main is not null)
                {
                    var position = e.GetCurrentPoint(_canvas).Position;
                    bool flag = true;
                    if (viewModel.Main.SelectedFigure is not null)
                    {
                        if (viewModel.Main.SelectedFigure.Model is BezierCurveModel && viewModel.Main.SelectTool == Tools.PenAdd)
                        {
                            viewModel.Main.History.Execute(new AddPointFigure(viewModel.Main.SelectedFigure, (float) position.X, (float) position.Y));
                            flag = false;
                        }
                        else if (viewModel.Main.SelectedFigure.Model is PolygonModel && viewModel.Main.SelectTool == Tools.PenAdd)
                        {
                            viewModel.Main.History.Execute(new AddPointFigure(viewModel.Main.SelectedFigure, (float) position.X, (float) position.Y));
                            flag = false;
                        }
                        else
                        {
                            viewModel.Main.SelectedFigure = null;
                        }
                    }

                    if (flag && viewModel.Main.SelectedButtonFigure is not null && viewModel.Main.SelectTool == Tools.SelectFigure)
                    {
                        viewModel.Main.SelectedButtonFigure.CreateCommand.Execute(position);
                        if ((viewModel.Main.SelectedButtonFigure.Factory.Metadata.Name == "BezierCurve" || viewModel.Main.SelectedButtonFigure.Factory.Metadata.Name == "Polygon") && viewModel.Main.Figures.Count > 0)
                        {
                            var figure = viewModel.Main.Figures[^1];
                            if (figure.Model is BezierCurveModel || figure.Model is PolygonModel)
                            {
                                viewModel.Main.SelectedFigure = figure;
                            }

                        }
                    }
                }

            }
        }

        private void MarkingPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is Canvas canvas && e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
            {
                var path = canvas.Children[0] as Path;
                var rect = canvas.Children[1] as Rectangle;
                // Получаем объект фигуры из DataContext
                if (path == null || rect == null)
                    return;
                if (path.DataContext is ShapeViewModel figure)
                {
                    // Устанавливаем выбранную фигуру в ViewModel
                    if (DataContext is CanvasViewModel viewmodel && viewmodel.Main is not null)
                    {
                        if (viewmodel.Main.SelectTool == Tools.Cursor)
                        {
                            if (viewmodel.Main.SelectedFigure is null || viewmodel.Main.SelectedFigure != figure)
                            {
                                viewmodel.Main.SelectedFigure = figure;
                            }
                            else
                            {
                                var position = e.GetPosition(rect);
                                var thickness = rect.StrokeThickness;
                                var width = rect.Width;
                                var height = rect.Height;


                                if (position.X <= thickness && position.Y <= thickness) // Левый верхний угол
                                {
                                    _isFigureResize = Operation.Rotate;
                                    deltaTheta = 0;
                                }
                                else if (position.X >= width - thickness && position.Y <= thickness) // Правый верхний угол
                                {
                                    _isFigureResize = Operation.Rotate;
                                    deltaTheta = 0;
                                }
                                else if (position.X <= thickness && position.Y >= height - thickness) // Левый нижний угол
                                {
                                    _isFigureResize = Operation.Rotate;
                                    deltaTheta = 0;
                                }
                                else if (position.X >= width - thickness && position.Y >= height - thickness) // Правый нижний угол
                                {
                                    _isFigureResize = Operation.Rotate;
                                    deltaTheta = 0;
                                }
                                // Проверка границ
                                else if (position.X <= thickness) // Левая граница
                                {
                                    _isFigureResize = Operation.ScaleX;
                                    _inversionScale = -1;
                                    deltaScaleX = 1;
                                }
                                else if (position.X >= width - thickness) // Правая граница
                                {
                                    _isFigureResize = Operation.ScaleX;
                                    _inversionScale = 1;
                                    deltaScaleX = 1;
                                }
                                else if (position.Y <= thickness) // Верхняя граница
                                {
                                    _isFigureResize = Operation.ScaleY;
                                    _inversionScale = -1;
                                    deltaScaleY = 1;
                                }
                                else if (position.Y >= height - thickness) // Нижняя граница
                                {
                                    _isFigureResize = Operation.ScaleY;
                                    _inversionScale = 1;
                                    deltaScaleY = 1;
                                }
                                else // Внутренняя область
                                {
                                    _isFigureResize = Operation.Translate;
                                    (_startpositionX, _startpositionY) = e.GetPosition(path);
                                    deltaY = 0; deltaY = 0;
                                }
                                if (_isFigureResize == Operation.Rotate)
                                {
                                    position = e.GetPosition(path);
                                    _startAngle = CalcAngle(position.X - figure.Model.CenterX, figure.Model.CenterY - position.Y);
                                }
                            }
                            e.Handled = true;
                        }
                        else if (viewmodel.Main.SelectTool == Tools.Pen)
                        {
                            if (viewmodel.Main.SelectedFigure is null && viewmodel.Main.SelectedFigure != figure)
                            {
                                viewmodel.Main.SelectedFigure = figure;
                            }
                            e.Handled = true;
                        }

                    }

                }
            }
        }

        private void MarkingMoved(object sender, PointerEventArgs e)
        {
            if (sender is Canvas canvas && DataContext is CanvasViewModel viewmodel && viewmodel.Main is not null)
            {
                if (_isFigureResize != Operation.None)
                {
                    canvas.Cursor = new Cursor(StandardCursorType.SizeAll);
                    // Устанавливаем выбранную фигуру в ViewModel
                    if (viewmodel.Main.SelectTool == Tools.Cursor)
                    {
                        if (viewmodel.Main.SelectedFigure is null)
                            return;
                        var newPosition = e.GetPosition(canvas);
                        var oldPositionX = viewmodel.Main.SelectedFigure.Model.CenterX;
                        var oldPositionY = viewmodel.Main.SelectedFigure.Model.CenterY;
                        double _deltaX, _deltaY, _angle, theta;
                        switch (_isFigureResize)
                        {
                            case Operation.Translate:
                                _deltaX = newPosition.X - _startpositionX;
                                _deltaY = newPosition.Y - _startpositionY;
                                _startpositionX = _startpositionX + _deltaX;
                                _startpositionY = _startpositionY + _deltaY;
                                deltaX += _deltaX;
                                deltaY += _deltaY;
                                viewmodel.Main.SelectedFigure.Model.Move((float) _deltaX, (float) _deltaY);
                                viewmodel.Main.SelectedFigure.UpdateCenterX();
                                viewmodel.Main.SelectedFigure.UpdateCenterY();
                                break;
                            case Operation.Rotate:
                                _deltaX = newPosition.X - oldPositionX;
                                _deltaY = newPosition.Y - oldPositionY;
                                theta = CalcAngle(_deltaX, _deltaY);
                                theta += _startAngle;
                                theta -= _oldTheta;
                                deltaTheta += theta;
                                _oldTheta += theta;
                                viewmodel.Main.SelectedFigure.Model.Rotate((float) theta);
                                viewmodel.Main.SelectedFigure.UpdateAngle();
                                break;
                            case Operation.ScaleX:
                                _angle = viewmodel.Main.SelectedFigure.Model.Angle;
                                if (_angle < 0)
                                    _angle += 360;
                                _deltaX = 2 * _inversionScale * (newPosition.X - oldPositionX) / float.Abs(viewmodel.Main.SelectedFigure.Model.Width);

                                if (_deltaX < 0)
                                {
                                    if (_isReflectionX)
                                    {
                                        var v = float.Abs((float) _deltaX);
                                        deltaScaleX *= v;
                                        viewmodel.Main.SelectedFigure.Model.Scale(v, (float) 1.0);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                    else
                                    {
                                        _isReflectionX = true;
                                        deltaScaleX *= (float) _deltaX;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) _deltaX, (float) 1.0);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                }
                                else if (_deltaX > 0)
                                {
                                    if (_isReflectionX)
                                    {
                                        _isReflectionX = false;
                                        deltaScaleX *= (float) -_deltaX;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) -_deltaX, (float) 1.0);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                    else
                                    {
                                        deltaScaleX *= (float) _deltaX;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) _deltaX, (float) 1.0);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                }
                                break;
                            case Operation.ScaleY:
                                _angle = viewmodel.Main.SelectedFigure.Model.Angle;
                                if (_angle < 0)
                                    _angle += 360;
                                _deltaY = 2 * _inversionScale * (newPosition.Y - oldPositionY) / float.Abs(viewmodel.Main.SelectedFigure.Model.Height);

                                if (_deltaY < 0)
                                {
                                    if (_isReflectionY)
                                    {
                                        var h = float.Abs((float) _deltaY);
                                        deltaScaleY *= h;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) 1.0, h);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                    else
                                    {
                                        _isReflectionY = true;
                                        deltaScaleY *= (float) _deltaY;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) 1.0, (float) _deltaY);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                }
                                else if (_deltaY > 0)
                                {
                                    if (_isReflectionY)
                                    {
                                        _isReflectionY = false;
                                        deltaScaleY *= (float) -_deltaY;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) 1.0, (float) -_deltaY);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                    else
                                    {
                                        deltaScaleY *= (float) _deltaY;
                                        viewmodel.Main.SelectedFigure.Model.Scale((float) 1.0, (float) _deltaY);
                                        viewmodel.Main.SelectedFigure.UpdateSize();
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        e.Handled = true;
                    }

                }
                else
                {
                    var rect = canvas.Children[1] as Rectangle;
                    if (rect is not null && rect.DataContext is ShapeViewModel figure && figure.Active)
                    {
                        if (viewmodel.Main.SelectTool == Tools.Cursor)
                        {
                            var position = e.GetPosition(rect);
                            var thickness = rect.StrokeThickness;
                            var width = rect.Width;
                            var height = rect.Height;

                            canvas.Cursor = null;
                            if (position.X <= thickness && position.Y <= thickness) // Левый верхний угол
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.Hand); // Курсор поворота
                            }
                            else if (position.X >= width - thickness && position.Y <= thickness) // Правый верхний угол
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.Hand); // Курсор поворота
                            }
                            else if (position.X <= thickness && position.Y >= height - thickness) // Левый нижний угол
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.Hand); // Курсор поворота
                            }
                            else if (position.X >= width - thickness && position.Y >= height - thickness) // Правый нижний угол
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.Hand); // Курсор поворота
                            }
                            // Проверка границ
                            else if (position.X <= thickness) // Левая граница
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.SizeWestEast);
                            }
                            else if (position.X >= width - thickness) // Правая граница
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.SizeWestEast);
                            }
                            else if (position.Y <= thickness) // Верхняя граница
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
                            }
                            else if (position.Y >= height - thickness) // Нижняя граница
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
                            }
                            else // Внутренняя область
                            {
                                canvas.Cursor = new Cursor(StandardCursorType.SizeAll);
                            }
                        }
                        else
                        {
                            canvas.Cursor = new Cursor(StandardCursorType.Arrow);
                        }
                        e.Handled = true;
                    }
                }

            }
        }

        private void MarkingReleased(object sender, RoutedEventArgs e)
        {
            if (DataContext is CanvasViewModel viewModel && viewModel.Main is not null && viewModel.Main.SelectedFigure is not null)
            {
                if (_isFigureResize == Operation.Translate)
                {
                    viewModel.Main.History.Add(new MoveFigure(viewModel.Main.SelectedFigure, (float) deltaX, (float) deltaY));
                }
                else if (_isFigureResize == Operation.Rotate)
                {
                    viewModel.Main.History.Add(new RotateFigure(viewModel.Main.SelectedFigure, (float) deltaTheta));
                }
                else if (_isFigureResize == Operation.ScaleX)
                    viewModel.Main.History.Add(new ScaleXFigure(viewModel.Main.SelectedFigure, (float) deltaScaleX));
                else if (_isFigureResize == Operation.ScaleY)
                    viewModel.Main.History.Add(new ScaleYFigure(viewModel.Main.SelectedFigure, (float) deltaScaleY));
            }


            deltaY = 0;
            deltaX = 0;
            deltaTheta = 0;
            deltaScaleX = 1;
            deltaScaleY = 1;
            _isFigureResize = Operation.None;
            _isReflectionX = false;
            _isReflectionY = false;
            _oldTheta = 0;
        }
        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas && DataContext is CanvasViewModel viewModel && viewModel.OriginalHeight > 0 && viewModel.OriginalWidth > 0)
            {
                // Устанавливаем Clip по размерам Canvas
                viewModel.MainCanvas = canvas;
                canvas.Clip = new RectangleGeometry(new Rect(0, 0, viewModel.OriginalWidth, viewModel.OriginalHeight));

            }
        }
        private void PointPress(object sender, PointerPressedEventArgs e)
        {
            if (sender is Ellipse ellipse && e.GetCurrentPoint(ellipse).Properties.IsLeftButtonPressed)
            {
                if (ellipse.DataContext is Point point && DataContext is CanvasViewModel viewmodel &&
                    viewmodel.Main is not null && viewmodel.Main.SelectedFigure is not null)
                {
                    if (viewmodel.Main.SelectTool == Tools.Pen)
                    {
                        // Устанавливаем выбранную фигуру в ViewModel

                        _isFigureResize = Operation.Translate;
                        (_startpositionX, _startpositionY) = point;
                        dynamic figure = viewmodel.Main.SelectedFigure.Model;
                        if (figure.GetType().GetProperty("ListOfPoints") != null)
                            _indexPoint = figure.ListOfPoints.IndexOf(point);
                        e.Handled = true;
                    }
                    else if (viewmodel.Main.SelectTool == Tools.PenRemove)
                    {
                        viewmodel.Main.History.Execute(new RemovePointFigure(viewmodel.Main.SelectedFigure, point));
                        e.Handled = true;
                    }
                }
            }
        }

        private void PointMove(object sender, PointerEventArgs e)
        {
            if (sender is Canvas marking && DataContext is CanvasViewModel figure && figure.Main is not null)
            {
                if (figure.Main.SelectedFigure is null)
                    return;
                if (figure.Main.SelectTool == Tools.Pen)
                {
                    if (_isFigureResize == Operation.Translate)
                    {
                        if (_indexPoint >= 0)
                        {
                            var newPosition = e.GetCurrentPoint(marking).Position;
                            var _deltaX = newPosition.X - _startpositionX;
                            var _deltaY = newPosition.Y - _startpositionY;

                            dynamic typefigure = figure.Main.SelectedFigure.Model;
                            if (typefigure.GetType().GetMethod("MovePoint") != null)
                            {
                                deltaX += _deltaX;
                                deltaY += _deltaY;
                                typefigure.MovePoint(_deltaX, _deltaY, _indexPoint);
                            }
                            dynamic Typefigure = figure.Main.SelectedFigure.Model;
                            if (Typefigure.GetType().GetProperty("ListOfPoints") != null)
                            {
                                var tempval = Typefigure.ListOfPoints[_indexPoint];
                                _startpositionX = tempval.X;
                                _startpositionY = tempval.Y;
                            }
                        }
                    }
                }
            }
        }
        private void MarkingPointMove(object sender, PointerEventArgs e)
        {
            if (sender is Ellipse marking && DataContext is CanvasViewModel figure && figure.Main is not null)
            {
                if (figure.Main.SelectedFigure is null)
                    return;
                if (figure.Main.SelectTool == Tools.Pen && figure.Main.SelectedFigure.Active)
                {
                    if (_isFigureResize == Operation.None)
                    {
                        marking.Cursor = new Cursor(StandardCursorType.SizeAll);
                    }
                }
                else if (figure.Main.SelectTool == Tools.PenRemove && figure.Main.SelectedFigure.Active && _isFigureResize == Operation.None)
                {
                    marking.Cursor = new Cursor(StandardCursorType.No);
                }
            }
        }
        private void PointReleased(object sender, RoutedEventArgs e)
        {
            if (DataContext is CanvasViewModel viewModel && viewModel.Main is not null && viewModel.Main.SelectedFigure is not null)
            {
                if (_isFigureResize == Operation.Translate)
                {
                    viewModel.Main.History.Add(new MovePointFigure(viewModel.Main.SelectedFigure, (float) deltaX, (float) deltaY, _indexPoint));
                    viewModel.Main.SelectedFigure.UpdateSize();
                    viewModel.Main.SelectedFigure.UpdateCenterY();
                    viewModel.Main.SelectedFigure.UpdateCenterX();
                }
            }

            deltaX = 0;
            deltaY = 0;
            _isFigureResize = Operation.None;
            _isReflectionX = false;
            _isReflectionY = false;
            _oldTheta = 0;
            _indexPoint = -1;
        }
    }
}