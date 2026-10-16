using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using GraphicsApp.ViewModels;

//using Tmds.DBus.Protocol;

namespace GraphicsApp.Views
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        [Obsolete]
        private async void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Ищем родительское окно, чтобы передать в диалог
            var parentWindow = this.FindAncestorOfType<Window>();
            if (parentWindow == null)
                return;

            var openFileDialog = new OpenFileDialog
            {
                Title = "Открыть файл",
                Directory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..")), // Исходная папка — папка с .csproj
                AllowMultiple = false, // Если нужно выбрать только один файл
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "JSON",
                        Extensions = new List<string> { "json" }
                    }
                }
            };

            // Показываем диалог и получаем выбранные файлы (массив путей)
            var result = await openFileDialog.ShowAsync(parentWindow);

            // Проверяем, выбрал ли пользователь хотя бы один файл
            if (result != null && result.Length > 0 && DataContext is SettingsViewModel viewModel)
            {
                string filePath = result[0];
                if (File.Exists(filePath))
                {
                    bool flag = true;
                    if (viewModel.Main.Figures.Count > 0)
                    {
                        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                        if (mainWindow is not null)
                            flag = await ShowMassageConfirmationDialog("Вы действительно хотите удалить текущие изменения?", mainWindow);
                    }
                    if (flag)
                    {
                        viewModel.Main.LoadJsonCommand.Execute(filePath);
                        viewModel.Main.Settingswindow.SaveLocation = filePath;
                    }
                }
            }
        }

        private async void OnSaveAsButtonClick(object sender, RoutedEventArgs e)
        {
            // Ищем окно, в котором находится данный UserControl
            var parentWindow = this.FindAncestorOfType<Window>();
            if (parentWindow == null)
                return;

            // Создаём диалог "Сохранить как"
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить файл как",
                Directory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..")), // Исходная папка — папка с .csproj
                InitialFileName = "NewFile", // Начальное имя файла
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "PNG files",
                        Extensions = new List<string> { "png" }
                    },
                    new FileDialogFilter
                    {
                        Name = "SVG files",
                        Extensions = new List<string> { "svg" }
                    },
                    new FileDialogFilter
                    {
                        Name = "JSON files",
                        Extensions = new List<string> { "json" }
                    }
                }
            };

            // Показываем диалог, передавая ему найденное окно
            var result = await saveFileDialog.ShowAsync(parentWindow);

            if (result != null && DataContext is SettingsViewModel viewModel)
            {
                var TypeFile = Path.GetExtension(result);
                var NameFile = Path.GetFileName(result);
                if (TypeFile != null && NameFile != null)
                {
                    var Type = TypeFile.ToLower();
                    switch (Type)
                    {
                        case ".json":
                            viewModel.Main.SaveJsonCommand.Execute(result);
                            break;
                        case ".svg":
                            viewModel.Main.SaveSvgCommand.Execute(result);
                            break;
                        case ".png":
                            viewModel.Main.SavePngCommand.Execute(result);
                            break;
                        default:
                            break;
                    }
                    if (File.Exists(result))
                        viewModel.Main.Settingswindow.SaveLocation = result;
                }
            }
        }

        private void OpenSettingsWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel viewModel)
            {
                SettingsWindow settings = new SettingsWindow { DataContext = viewModel.Main.Settingswindow };
                settings.Show();
            }
        }
        public async Task<bool> ShowMassageConfirmationDialog(string text, Window currwindow)
        {
            var dialog = new MessageBox();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            await dialog.ShowDialog(currwindow); // MainWindow - ссылка на главное окно
            return dialog.Result;
        }
        private async void CreateButton_Pressed(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel viewModel && viewModel.Main is not null)
            {
                bool flag = true;
                if (viewModel.Main.Figures.Count > 0)
                {
                    var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                    if (mainWindow is not null)
                        flag = await ShowMassageConfirmationDialog("Вы действительно хотите удалить текущие изменения?", mainWindow);
                }
                if (flag)
                    viewModel.Main.ToDefaultSettingsAndClearCanvas.Execute(null);
            }
        }
        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel viewModel && viewModel.Main is not null)
            {
                var result = viewModel.Main.Settingswindow.SaveLocation;
                var TypeFile = Path.GetExtension(result);
                var NameFile = Path.GetFileName(result);
                if (TypeFile != null && NameFile != null)
                {
                    var Type = TypeFile.ToLower();
                    switch (Type)
                    {
                        case ".json":
                            viewModel.Main.SaveJsonCommand.Execute(result);
                            break;
                        case ".svg":
                            viewModel.Main.SaveSvgCommand.Execute(result);
                            break;
                        case ".png":
                            viewModel.Main.SavePngCommand.Execute(result);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var dialog = new MessageBoxOk();
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                    if (mainWindow is not null)
                        await dialog.ShowDialog(mainWindow);
                }
            }
        }
    }
}