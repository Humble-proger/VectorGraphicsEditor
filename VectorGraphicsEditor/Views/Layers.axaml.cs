using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;


using GraphicsApp.ViewModels;

namespace GraphicsApp.Views
{
    public partial class Layers : UserControl
    {
        public float Help = 1;

        public Layers()
        {
            InitializeComponent();

            //DataContext = this; // Устанавливаем контекст данных


        }
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is LayersViewModel viewmodel && viewmodel.Main is not null)
            {
                if (viewmodel.Main.SelectedFigure is not null)
                    viewmodel.Main.SelectedFigure.Remove.Execute(null);
                viewmodel.Main.SelectedFigure = null;
            }

        }

        private void OnFillButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is LayersViewModel viewmodel && viewmodel.Main is not null)
            {
                if (viewmodel.Main.SelectedFigure is not null)
                    if (sender is CheckBox myButton && myButton.IsChecked is not null)
                    {
                        if (myButton.IsChecked.Value)
                        {
                            var originalColor = viewmodel.Main.SelectedFigure.Model.Fill;
                            var newColor = Color.FromArgb(255, originalColor.R, originalColor.G, originalColor.B);
                            viewmodel.Main.SelectedFigure.Fill = newColor;
                        }
                        else
                        {
                            var originalColor = viewmodel.Main.SelectedFigure.Model.Fill;
                            var newColor = Color.FromArgb(0, originalColor.R, originalColor.G, originalColor.B);
                            viewmodel.Main.SelectedFigure.Fill = newColor;
                        }
                    }
            }
        }


        private void MoveUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is LayersViewModel viewmodel && viewmodel.Main is not null)
            {
                if (viewmodel.Main.SelectedFigure is not null)
                    viewmodel.MoveUpFigure.Execute(viewmodel.Main.SelectedFigure);
            }
        }

        private void MoveDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is LayersViewModel viewmodel && viewmodel.Main is not null)
            {
                if (viewmodel.Main.SelectedFigure is not null)
                    viewmodel.MoveDownFigure.Execute(viewmodel.Main.SelectedFigure);
            }
        }
    }
}