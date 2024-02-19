using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MusicCreator.UI;

public partial class MusicExplorer : UserControl
{
    private Action showDashboardAction;
    public ObservableCollection<string> Items;
    public MusicExplorer(Action showDashboard)
    {
        InitializeComponent();
        showDashboardAction = showDashboard;
        DataContext = new MusicExplorerViewModel();
        
        
    }
    
    private void ShowDashboard(object sender, RoutedEventArgs args)
    {
        showDashboardAction();
    }

    private void PlaySound(object sender, RoutedEventArgs args)
    {
        var tag = ((Button)sender).Tag?.ToString() ?? "";
        PlaySound(tag);
    }
    
    private static void PlaySound(string fileName)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var basePath = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? "";
        var scriptPath = basePath + "/Scripts/playSound.sh";
        var musicPath = basePath + $"/Sounds/{fileName}";
        var startInfo = new ProcessStartInfo() { FileName = scriptPath, Arguments = musicPath };
        var proc = new Process() { StartInfo = startInfo };
        proc.Start();
    }
}