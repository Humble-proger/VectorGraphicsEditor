using System;


using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GraphicsApp.Views
{
    public partial class Save_As : UserControl
    {
        public Save_As()
        {
            InitializeComponent();
        }

        // Метод для открытия Popup
        public void OpenPopup()
        {
            Save_AsPopup.IsOpen = true;
        }

        // Метод для закрытия Popup
        public void ClosePopup()
        {
            Save_AsPopup.IsOpen = false;
        }

        // Обработчик события нажатия на кнопку
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            ClosePopup(); // Закрываем Popup при нажатии на кнопку
        }
    }
}