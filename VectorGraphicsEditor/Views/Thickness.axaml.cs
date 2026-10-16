using System;

using Avalonia.Controls;
using Avalonia.Interactivity;

using GraphicsApp.ViewModels;

namespace GraphicsApp.Views
{
    public partial class Thickness : UserControl
    {
        public Thickness()
        {
            InitializeComponent();
            //DataContext = this;
        }

        public event EventHandler<int> ThicknessChanged;

        private void ApplyThickness(object sender, RoutedEventArgs e)
        {
            int thickness = (int) ThicknessSlider.Value;
            ThicknessChanged?.Invoke(this, thickness);
            // ThicknessFlyout.Hide(); // Теперь это должно быть удалено
            // Теперь надо закрыть Flyout через ThicknessButton из ToolBars
            // Например, можно вызвать событие, которое обработает ToolBars
            OnCloseFlyoutRequested();
        }

        public event EventHandler CloseFlyoutRequested;

        protected virtual void OnCloseFlyoutRequested()
        {
            CloseFlyoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}