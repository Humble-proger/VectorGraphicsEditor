using CommunityToolkit.Mvvm.ComponentModel;

namespace GraphicsApp.ViewModels
{
    public partial class MessageBoxViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _textMessage;
    }
}