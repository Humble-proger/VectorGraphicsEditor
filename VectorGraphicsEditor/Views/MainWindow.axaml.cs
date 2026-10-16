using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GraphicsApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(); // Создаем экземпляр SettingsWindow
            await settingsWindow.ShowDialog(this);       // Открываем как диалоговое окно, this - родитель
        }

    }
}