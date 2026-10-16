using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GraphicsApp.Views
{
    public partial class MessageBoxOk : Window
    {
        public bool Result { get; private set; } = false;
        public MessageBoxOk()
        {
            InitializeComponent();
        }
        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}