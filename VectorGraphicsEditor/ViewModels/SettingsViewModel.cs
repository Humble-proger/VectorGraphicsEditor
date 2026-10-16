using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;


namespace GraphicsApp.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private MainWindowViewModel _main;

    public SettingsViewModel(MainWindowViewModel main)
    {
        Main = main;
    }

}