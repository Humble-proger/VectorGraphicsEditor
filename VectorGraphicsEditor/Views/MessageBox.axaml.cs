using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GraphicsApp.Views
{
    public partial class MessageBox : Window
    {
        public bool Result { get; private set; } = false;
        public MessageBox()
        {
            InitializeComponent();
        }
        private void OnYesClicked(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void OnNoClicked(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}